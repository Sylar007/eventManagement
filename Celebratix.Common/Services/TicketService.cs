using AutoMapper;
using Celebratix.Common.Configs;
using Celebratix.Common.Database;
using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Models.DTOs.Business.Tickets;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Models.DTOs.User.Tickets;
using Celebratix.Common.Services.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Celebratix.Common.Models.Enums;
using F = CSharpFunctionalExtensions;
using Humanizer;
using LinqKit;

namespace Celebratix.Common.Services;

public class TicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TicketScanningConfig _ticketConfig;
    private readonly MarketplaceConfig _marketplaceConfig;
    private readonly ILogger<TicketService> _logger;
    private readonly TicketTransferService _ticketTransferService;
    private readonly IImageStorageService _imageStorageService;
    private readonly IValidator<EventTicketCreateRequest> _eventTicketCreateRequestValidator;
    private readonly IValidator<TicketUpdateRefundInputDto> _ticketUpdateRefundInputDtoValidator;
    private enum OperationType
    {
        Scan = 1,
        Refund = 2
    }
    private IMapper _mapper;

    public TicketService(
        ILogger<TicketService> logger,
        IOptions<TicketScanningConfig> ticketConfig,
         IOptions<MarketplaceConfig> marketplaceConfig,
        TicketTransferService ticketTransferService,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorageService,
        IValidator<EventTicketCreateRequest> eventTicketCreateRequestValidator,
        IValidator<TicketUpdateRefundInputDto> ticketUpdateRefundInputDtoValidator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _marketplaceConfig = marketplaceConfig.Value;
        _ticketConfig = ticketConfig.Value;
        _ticketTransferService = ticketTransferService;
        _imageStorageService = imageStorageService;
        _eventTicketCreateRequestValidator = eventTicketCreateRequestValidator;
        _ticketUpdateRefundInputDtoValidator = ticketUpdateRefundInputDtoValidator;
        _mapper = mapper;
    }

    public async Task<TicketQrDto> GetTicketForOwner(string ownerId, Guid id)
    {
        var ticket = await _unitOfWork.DbContext.Tickets
            .Where(t => t.Id == id)
            .Include(t => t.TicketType!.Event)
            .FirstOrThrowAsync();

        return new TicketQrDto(ticket, ownerId, GetTicketKey(ticket));
    }

    public async Task<TicketDto> GetTicketDetailsForOwner(string ownerId, Guid id)
    {
        var ticket = await _unitOfWork.DbContext.Tickets
            .Where(t => t.Id == id)
            .Where(t => t.OwnerId == ownerId)
            .Include(t => t.TicketType!.Event!.Image)
            .Include(t => t.TicketType!.Event!.Category)
            .Include(t => t.TicketType!.Event!.Currency)
            .Include(t => t.OriginalOrder)
            .Select(t => new TicketDto(t, _marketplaceConfig.MaxPriceOverOriginal))
            .FirstOrThrowAsync();

        return ticket;
    }

    public async Task<List<TicketQrDto>> GetUsersTicketsForOrder(string userId, Guid orderId, bool excludeUnavailable)
    {
        var tickets = await _unitOfWork.DbContext.Orders
            .Where(o => o.PurchaserId == userId)
            .Where(o => o.Id == orderId)
            .SelectMany(o => o.Tickets!)
            .Include(t => t.TicketType!.Event)
            .ToListAsync();

        if (!tickets.Any())
        {
            // Just to throw the correct exception if the list is empty
            await _unitOfWork.DbContext.Orders
                .Where(o => o.PurchaserId == userId)
                .Where(o => o.Id == orderId)
                .AnyOrThrowAsync();
        }

        if (excludeUnavailable)
        {
            tickets = tickets.Where(t => t.OwnerId == userId).ToList(); // FIXME: This is a hotfix for https://celebratixio.atlassian.net/jira/software/projects/CLBTX/boards/2?selectedIssue=CLBTX-123
        }

        var ticketDtos = new List<TicketQrDto>();

        foreach (var ticket in tickets)
        {
            var dto = new TicketQrDto(ticket, userId, GetTicketKey(ticket));
            ticketDtos.Add(dto);
        }
        return ticketDtos;
    }

    public async Task<PagedResultDto<TicketDto>> GetAllUsersTicketsForUpcomingEvents(string userId, int page, int pageSize)
    {
        return await _unitOfWork.DbContext.Tickets
            .Include(t => t.TicketType!.Event!.Image)
            .Include(t => t.TicketType!.Event!.Category)
            .Include(t => t.TicketType!.Event!.Currency)
            .Include(t => t.OriginalOrder)
            .Where(t => t.TicketType!.Event!.EndDate > DateTimeOffset.UtcNow)
            .Where(t => t.OwnerId == userId)
            .Where(t => !t.CheckedIn)
            .Where(t => t.ActiveMarketplaceListingId == null)
            .Where(t => t.ActiveTicketTransferOfferId == null)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TicketDto(t, _marketplaceConfig.MaxPriceOverOriginal))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);
    }

    public async Task<TicketScanningResponseDto> ScanTicket(int eventId, TicketScanningInputDto scanningRequest)
    {
        return await ValidateAndUpdateTicket(eventId, scanningRequest, OperationType.Scan);
    }

    public async Task<TicketScanningResponseDto> RefundTicket(int eventId, TicketScanningInputDto scanningRequest)
    {
        return await ValidateAndUpdateTicket(eventId, scanningRequest, OperationType.Refund);
    }

    private async Task<TicketScanningResponseDto> ValidateAndUpdateTicket(int eventId, TicketScanningInputDto scanningRequest, OperationType operationType)
    {
        var oldestAllowedTimestamp = DateTimeOffset.UtcNow.AddSeconds(-_ticketConfig.ValidForSeconds);
        if (oldestAllowedTimestamp > scanningRequest.Timestamp)
        {
            throw new TicketQrTimeExpiredException();
        }

        var pair = DecryptTicketOwnerPair(_ticketConfig.AesKey, scanningRequest.CipherText);
        if (pair.HasNoValue)
            throw new Exception("Invalid encrypted data format");
        var (ticketId, userId) = pair.Value;

        await using var transaction = await _unitOfWork.CreateTransactionAsync();

        var ownerName = (await _unitOfWork.DbContext.Users.FirstOrThrowAsync(u => u.Id == userId)).FullName;

        var ticket = await _unitOfWork.DbContext.Tickets
            .Include(t => t.TicketType!.Event)
            .Where(t => t.OwnerId == userId)
            .FirstOrThrowAsync(t => t.Id == ticketId);

        if (ticket.TicketType!.EventId != eventId)
        {
            throw new TicketWrongEventException();
        }

        VerifyTicketCanBeScanned(ticket);

        if (scanningRequest.AllowedTicketTypes != null && scanningRequest.AllowedTicketTypes.Any())
        {
            if (!scanningRequest.AllowedTicketTypes.Contains(ticket.TicketTypeId))
            {
                throw new TicketBadTicketTypeException($"Ticket is of type: {ticket.TicketType.Name}");
            }
        }

        if (operationType == OperationType.Scan)
        {
            ticket.TicketType.TicketsCheckedIn++;
            ticket.CheckedIn = true;
            await _ticketTransferService.CancelTransferOffersForTicket(ticket.Id, TicketTransferService.CancelReason.TicketScanned);
            await transaction.CompleteAsync();

            _logger.LogInformation("Successfully scanned ticket with id: {TicketId} for user with id {UserId}", ticketId, userId);
        }
        else if (operationType == OperationType.Refund)
        {
            VerifyTicketIsValidToRefund(ticket);

            ticket.Refund = Enums.RefundType.WaitingForReview;
            await _ticketTransferService.CancelTransferOffersForTicket(ticket.Id, TicketTransferService.CancelReason.TicketScanned);
            await transaction.CompleteAsync();

            _logger.LogInformation("Successfully marked {Status} for ticket with id: {TicketId} for user with id {UserId}", nameof(RefundType.WaitingForReview), ticketId, userId);
        }

        return new TicketScanningResponseDto(ticket, ownerName);
    }

    /// <summary>
    /// Throws if the ticket is already refund or waiting for review, accepted or denied
    /// </summary>
    public static void VerifyTicketIsValidToRefund(Ticket ticket)
    {
        switch (ticket.Refund)
        {
            case Enums.RefundType.WaitingForReview:
                throw new TicketForRefundWaitingReviewException();
            case Enums.RefundType.Approved:
                throw new TicketForRefundApprovedException();
            case Enums.RefundType.Denied:
                throw new TicketForRefundDeniedException();
        }
    }

    public async Task<Result<PagedResultDto<TicketResponseDto>>> GetTicketsBySearch(TicketSearchRefundInputDto searchInputDto,
        Guid businessId, int page, int pageSize)
    {
        if (searchInputDto == null)
        {
            return await _unitOfWork.DbContext.Tickets
                .Include(e => e.TicketType)
                .Include(e => e.Owner)
                .Where(ts => ts.Owner!.BusinessId == businessId)
                .Select(ts => new TicketResponseDto(ts))
                .ToPagedResult(1, 10);
        }

        var querySearch = GetEventQuery(searchInputDto);
        var querySort = GetSortQuery();

        return await querySort
            .Include(e => e.TicketType)
            .Include(e => e.Owner)
            .Where(e => e.Owner!.BusinessId == businessId)
            .Where(querySearch)
            .Select(e => new TicketResponseDto(e))
            .ToPagedResult(page, pageSize);
    }

    private static ExpressionStarter<Ticket> GetEventQuery(TicketSearchRefundInputDto searchInputDto)
    {
        var predicate = PredicateBuilder.New<Ticket>(false);

        if (searchInputDto.Statuses != null)
        {
            foreach (var status in searchInputDto.Statuses)
            {
                predicate = predicate.Or(e => e.Refund == status);
            }
        }
        if (searchInputDto.EventIds != null)
        {
            foreach (var eventId in searchInputDto.EventIds)
            {
                predicate = predicate.Or(e => e.TicketType!.EventId == eventId);
            }
        }

        return predicate;
    }

    private IQueryable<Ticket> GetSortQuery()
    {
        IQueryable<Ticket> tickets;
        tickets = _unitOfWork.DbContext.Tickets.AsQueryable().OrderBy(e => e.CreatedAt);
        return tickets;
    }

    public async Task<Result<Ticket>> UpdateRefundTicket(TicketUpdateRefundInputDto request)
    {
        var validationResult = await _ticketUpdateRefundInputDtoValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.ToErrorMessage());
        }

        var eventEntity = await _unitOfWork.DbContext.Tickets
                                 .FirstOrThrowAsync(e => e.Id == request.Id);
        if (eventEntity == null)
            return new Ticket();

        eventEntity.Refund = request.Status;
        _unitOfWork.DbContext.Tickets.Update(eventEntity);

        await _unitOfWork.DbContext.SaveChangesAsync();

        _logger.LogInformation("Successful updated status for Ticket id: {TicketId} with Status: {Status}.", eventEntity.Id, eventEntity.Refund);

        return eventEntity;
    }

    public async Task<F.Result<TicketScanningResponseDto, ErrEnum<ScanFailReason>>> ScanTicketV2(int eventId, TicketScanningInputDto dto)
    {
        var result = await _ScanTicketV2(eventId, dto);
        var logStr = $"{(result.IsSuccess ? "Successfully" : "Failed to")} scan ticket\n" +
            $"EventId        : {eventId}\n" +
            $"Timestamp      : {dto.Timestamp.UtcDateTime.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}\n" +
            $"Now - timestamp: {(DateTimeOffset.UtcNow - dto.Timestamp.UtcDateTime).Humanize(3)}\n" +
            $"CipherText     : {dto.CipherText}";

        if (result.IsSuccess)
            _logger.LogInformation(logStr);
        else
            _logger.LogWarning($"{logStr}\nReason: {result.Error}");
        return result;
    }

    private async Task<F.Result<TicketScanningResponseDto, ErrEnum<ScanFailReason>>> _ScanTicketV2(int eventId, TicketScanningInputDto dto)
    {
        var oldestAllowedTimestamp = DateTimeOffset.UtcNow.AddSeconds(-_ticketConfig.ValidForSeconds);
        var diff = oldestAllowedTimestamp - dto.Timestamp;
        if (diff > TimeSpan.Zero)
            return ErrEnum.New(ScanFailReason.QrTimeExpired, $"Ticket qr code is not up to date by {diff.Humanize(3)}");

        var pair = DecryptTicketOwnerPair(_ticketConfig.AesKey, dto.CipherText);
        if (pair.HasNoValue)
            return ErrEnum.New(ScanFailReason.QrCorrupted, "Ticket qr data is corrupted");
        var (ticketId, userId) = pair.Value;

        await using var transaction = await _unitOfWork.CreateTransactionAsync();

        var ownerName = (await _unitOfWork.DbContext.Users.FirstOrDefaultAsync(u => u.Id == userId))?.FullName;
        if (ownerName == null)
            return ErrEnum.New(ScanFailReason.UserNotFound, $"User with id {userId} does not exist");

        var ticket = await _unitOfWork.DbContext.Tickets
            .Include(t => t.TicketType!.Event)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
            return ErrEnum.New(ScanFailReason.TicketNotFound, $"Ticket with id {ticketId} does not exist");


        if (ticket.TicketType!.EventId != eventId)
            return ErrEnum.New(ScanFailReason.ForbiddenEvent,
             $"Ticket is is for another event: {ticket.TicketType.Event?.Name} {ticket.TicketType.Event?.StartDate}");

        var qrReason = new TicketQrActions(ticket, userId, "").Qr.UnavailableReason;
        if (qrReason != null)
        {
#pragma warning disable 8524 // disabling this rule guarantees exhaustive switch
            return qrReason switch
#pragma warning restore 8524
            {
                QrUnavailableReason.CheckedIn => ErrEnum.New(ScanFailReason.CheckedIn, "Ticket has already been checked in"), // TODO: when?
                QrUnavailableReason.SoldOrTransferred => ErrEnum.New(ScanFailReason.SoldOrTransferred, "Ticket has been sold or transferred"),
                QrUnavailableReason.EventPassed => ErrEnum.New(ScanFailReason.EventPassed, "The event has passed"),
                QrUnavailableReason.MarketplaceListing => ErrEnum.New(ScanFailReason.MarketplaceListing, "Ticket is listed on the marketplace"),
            };
        }

        if (dto.AllowedTicketTypes != null && dto.AllowedTicketTypes.Any())
        {
            if (!dto.AllowedTicketTypes.Contains(ticket.TicketTypeId))
                return ErrEnum.New(ScanFailReason.ForbiddenTicketType,
                 $"Ticket with {ticket.TicketType.Name} is at the wrong entrance)");
        }

        ticket.TicketType.TicketsCheckedIn++;
        ticket.CheckedIn = true;

        await _ticketTransferService.CancelTransferOffersForTicket(ticket.Id, TicketTransferService.CancelReason.TicketScanned);

        await transaction.CompleteAsync();
        return new TicketScanningResponseDto(ticket, ownerName);
    }

    /// <summary>
    /// Returns all the events for which the user owns at least 1 ticket
    /// </summary>
    public async Task<PagedResultDto<EventWithTicketDataDto>> GetEventsByOwnedTickets(string ownerId, Enums.EventStatus? eventStatus, int page, int pageSize)
    {
        var query = _unitOfWork.DbContext.Tickets
            .Where(t => t.OwnerId == ownerId)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Category!)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Image!)
            .Select(t => t.TicketType!.Event!)
            .GroupBy(e => e.Id);
        query = eventStatus switch
        {
            Enums.EventStatus.Upcoming => query.Where(e => e.Distinct().Single().EndDate >= DateTimeOffset.UtcNow),
            Enums.EventStatus.Past => query.Where(e => DateTimeOffset.UtcNow >= e.Distinct().Single().EndDate),
            _ => query
        };
        return await query
            .Select(e => new EventWithTicketDataDto(e.First(), e.Count()))
            .ToPagedResult(page, pageSize);
    }

    /// <summary>
    /// Returns all the tickets the user owns of a specific ticketType
    /// </summary>
    public async Task<List<TicketQrDto>> GetOwnedTicketsForTicketType(string ownerId, Guid ticketTypeId)
    {
        return await _unitOfWork.DbContext.Tickets
            .Where(t => t.OwnerId == ownerId)
            .Where(t => t.TicketTypeId == ticketTypeId)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Category!)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Image!)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Currency!)
            .Include(t => t.OriginalOrder)
            .Select(t => new TicketQrDto(t, ownerId, GetTicketKey(t)))
            .ToListAsync();
    }

    /// <summary>
    /// Returns all the tickets the user owns for a specific event
    /// </summary>
    public async Task<List<TicketQrDto>> GetOwnedTicketsForEvent(string ownerId, int eventId)
    {
        return await _unitOfWork.DbContext.Tickets
            .Where(t => t.OwnerId == ownerId)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Category!)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Image!)
            .Include(t => t.TicketType!).ThenInclude(tt => tt.Event!).ThenInclude(e => e.Currency!)
            .Include(t => t.OriginalOrder)
            .Where(t => t.TicketType!.EventId == eventId)
            .OrderBy(t => t.TicketType!.SortIndex)
            .Select(t => new TicketQrDto(t, ownerId, GetTicketKey(t)))
            .ToListAsync();
    }

    public static void VerifyTicketCanBeScanned(Ticket ticket)
    {
        var action = new TicketQrActions(ticket, ticket.OwnerId!, "");
        switch (action.Qr.UnavailableReason)
        {
            case null:
                break;
            case QrUnavailableReason.CheckedIn:
                throw new TicketAlreadyScannedException();
            case QrUnavailableReason.SoldOrTransferred:
                throw new TicketSoldOrTransferredException();
            case QrUnavailableReason.MarketplaceListing:
                throw new TicketInMarketplaceListingException();
            case QrUnavailableReason.EventPassed:
                throw new EventEndTimePassedException();
            default:
                throw new Exception($"Unknown {nameof(QrUnavailableReason)}: {action.Qr.UnavailableReason}");
        };
    }

    /// <summary>
    /// Returns the string that is used to generate the QR code
    /// </summary>
    private string GetTicketKey(Ticket ticket)
    {
        // Convert ticketId to string and append ownerId
        var plainText = $"{ticket.Id}|{ticket.OwnerId!}";

        return AesHelper.EncryptString(_ticketConfig.AesKey, plainText);
    }

    private static F.Maybe<(Guid, string)> DecryptTicketOwnerPair(string key, string cipherText)
    {
        var decryptedText = AesHelper.DecryptString(key, cipherText);
        if (decryptedText.HasNoValue)
            return F.Maybe.None;
        // Split the decrypted text back into ticketId and ownerId
        var parts = decryptedText.Value.Split('|');
        if (parts.Length != 2 || !Guid.TryParse(parts[0], out var ticketId))
            return F.Maybe.None;

        return (ticketId, parts[1]);
    }

    public async Task<Result<TicketTypeDashboardDto>> CreateEventTicket(EventTicketCreateRequest request)
    {
        ImageDbModel? dbImage = null;

        try
        {
            var validationResult = await _eventTicketCreateRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.ToErrorMessage());
            }

            var eventEntity = await _unitOfWork.DbContext.Events
                .Where(e => e.Id == request.EventId)
                .SingleOrDefaultAsync();

            if (eventEntity == null)
            {
                return Result.Fail(new CelebratixError(ErrorCode.CelebratixEventInvalidOrNotFound, "Event is invalid or not found"));
            }

            var eventTicketType = _mapper.Map<EventTicketCreateRequest, EventTicketType>(request);
            dbImage = await _imageStorageService.UploadFile(request.Image);
            if (dbImage != null)
            {
                _unitOfWork.DbContext.Add(dbImage);
                eventTicketType.Image = dbImage;
            }

            _unitOfWork.DbContext.Add(eventTicketType);
            await _unitOfWork.DbContext.SaveChangesAsync();
            _logger.LogInformation("Ticket type for event {EventId} with id {TicketTypeId} was created",
                request.EventId, eventTicketType.Id);

            //TODO: use mapper to create the DTO
            return new TicketTypeDashboardDto(eventTicketType);
        }
        catch
        {
            if (dbImage != null)
            {
                await _imageStorageService.DeleteFile(dbImage.StorageIdentifier);
            }
            throw;
        }
    }
}
