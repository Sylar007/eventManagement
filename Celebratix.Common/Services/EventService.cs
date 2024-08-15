using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Services.Interfaces;
using FluentResults;
using FluentValidation;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System.Data;
using System.Linq.Expressions;

namespace Celebratix.Common.Services;

public class EventService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly UserService _userService;
    private readonly IImageStorageService _imageStorageService;
    private readonly ChannelService _channelService;
    private readonly OrderAggregatorService _orderAggregatorService;
    private readonly ILogger<EventService> _logger;
    private readonly AffiliateCodeService _affiliateCodeService;
    private readonly IValidator<EventCreateRequest> _eventCreateRequestValidator;
    private readonly IValidator<EventUpdateRequest> _eventUpdateRequestValidator;
    private readonly IValidator<ChannelEventUpdateResaleRequest> _channelEventUpdateResaleRequestValidator;
    private const int EventViewCodeLength = 14;

    public EventService(
        CelebratixDbContext dbContext,
        ILogger<EventService> logger,
        UserService userService,
        IImageStorageService imageStorageService,
        ChannelService channelService,
        AffiliateCodeService affiliateCodeService,
        OrderAggregatorService orderAggregatorService,
        IValidator<EventCreateRequest> eventCreateRequestValidator,
        IValidator<EventUpdateRequest> eventUpdateRequestValidator,
        IValidator<ChannelEventUpdateResaleRequest> channelEventUpdateResaleRequestValidator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userService = userService;
        _imageStorageService = imageStorageService;
        _channelService = channelService;
        _affiliateCodeService = affiliateCodeService;
        _orderAggregatorService = orderAggregatorService;
        _eventCreateRequestValidator = eventCreateRequestValidator;
        _eventUpdateRequestValidator = eventUpdateRequestValidator;
        _channelEventUpdateResaleRequestValidator = channelEventUpdateResaleRequestValidator;
    }

    public async Task<PagedResultDto<EventBasicDto>> GetEventsAsUserDtos(int page, int pageSize, string? searchString, string? channelSlug)
    {
        var baseQuery = _dbContext.VisibleEvents
            .Where(e => e.EndDate >= DateTimeOffset.UtcNow);

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            baseQuery = baseQuery.Where(e =>
                e.Name.Contains(searchString)
                || e.Business!.Name.Contains(searchString)
                || e.Location!.Contains(searchString));
        }

        // If the channel slug does not exist, we ignore the it
        if (!string.IsNullOrWhiteSpace(channelSlug) &&
            await _dbContext.Channels.AnyAsync(c => c.Slug == channelSlug))
        {
            baseQuery = baseQuery
                .Include(e => e.Channels)
                .Where(e => e.Channels.Any(c => c.Slug == channelSlug));
        }

        return await baseQuery
            .OrderBy(e => e.StartDate)
            .Include(e => e.Category)
            .Include(e => e.Image)
            .Select(e => new EventBasicDto(e))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);
    }

    private async Task<EventDetailedDto> GetEventAsUserDto(Expression<Func<Event, bool>> predicate, string? ticketTypeLinkCode, string? affiliateCodeCode)
    {
        affiliateCodeCode = string.IsNullOrWhiteSpace(affiliateCodeCode) ? null : affiliateCodeCode;
        if (affiliateCodeCode != null && !await _dbContext.Affiliates.AnyAsync(ac => ac.Code == affiliateCodeCode))
        {
            _logger.LogWarning($"Affiliate code {affiliateCodeCode} does not exist");
            affiliateCodeCode = null;
        }
        ticketTypeLinkCode = string.IsNullOrWhiteSpace(ticketTypeLinkCode) ? null : ticketTypeLinkCode;

        var baseQuery = _dbContext.VisibleEvents
            .Include(e => e.Business)
            .Include(e => e.Category)
            .Include(e => e.Currency)
            .Include(e => e.Image)
            .Include(e => e.TicketBackgroundImage)
            .Include(e => e.TicketTypes!
                .Where(tt => (ticketTypeLinkCode == null && affiliateCodeCode == null && tt.LinkCode == null && !tt.OnlyAffiliates && tt.PubliclyAvailable) || // fully public
                             (tt.LinkCode != null && tt.LinkCode == ticketTypeLinkCode) || // only if link code matches
                             (tt.OnlyAffiliates && affiliateCodeCode != null))) // only if valid affiliate code is provided
            .AsSplitQuery();

        if (affiliateCodeCode != null)
            baseQuery = baseQuery.Include(e => e.Channels);

        var eventEntity = await baseQuery.FirstOrThrowAsync(predicate);
        var @event = new EventDetailedDto(eventEntity);

        if (affiliateCodeCode == null)
            return @event;

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        Affiliate affiliateCode;
        try
        {
            affiliateCode = await _affiliateCodeService.GetAffiliateCodeByCode(affiliateCodeCode!);
        }
        catch (Exception)
        {
            _logger.LogWarning($"Affiliate code {affiliateCodeCode} does not exist");
            return @event;
        }

        var isChannelValid = eventEntity.Channels.Any(c => c.Id == affiliateCode.ChannelId);
        if (!isChannelValid)
            return @event;

        affiliateCode.Views++;
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return @event;
    }

    public async Task<EventDetailedDto> GetEventByIdAsUserDto(int id, string? ticketTypeLinkCode, string? affiliateCode)
    {
        return await GetEventAsUserDto(e => e.Id == id, ticketTypeLinkCode, affiliateCode);
    }

    public async Task<EventDetailedDto> GetEventBySlugAsUserDto(string customSlug, string? ticketTypeLinkCode, string? affiliateCode)
    {
        return await GetEventAsUserDto(e => e.CustomSlug == customSlug, ticketTypeLinkCode, affiliateCode);
    }

    private IQueryable<Event> GetEventBase()
    {
        return _dbContext.Events
            .Include(e => e.Business)
            .Include(e => e.Category)
            .Include(e => e.Channels)
            .Include(e => e.Creator)
            .Include(e => e.Currency)
            .Include(e => e.Image)
            .Include(e => e.TicketBackgroundImage)
            .Include(e => e.TicketTypes);
    }

    public async Task<PagedResultDto<EventBasicBusinessDto>> GetEventsAsBusinessDtos(Guid businessId, int page,
    int pageSize)
    {
        return await GetEventBase()
            .OrderByDescending(e => e.StartDate)
            .Where(e => e.BusinessId == businessId)
            .Select(e => new EventBasicBusinessDto(e))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);
    }

    public async Task<Result<PagedResultDto<EventBasicBusinessDto>>> GetEventsBySearch(Guid businessId, EventSearchRequest? searchRequest)
    {
        if (searchRequest == null)
        {
            return await _dbContext.Events
                .Where(ts => ts.BusinessId == businessId)
                .Select(ts => new EventBasicBusinessDto(ts))
                .ToPagedResult(1, 10);
        }

        var querySearch = GetEventSearch(searchRequest);
        var queryFilter = GetFilterQuery(searchRequest);
        var querySort = GetSortQuery(searchRequest);

        return await querySort
            .Include(e => e.TicketTypes)
            .Where(e => e.BusinessId == businessId)
            .Where(querySearch)
            .Where(queryFilter)
            .Select(e => new EventBasicBusinessDto(e))
            .ToPagedResult(searchRequest.Page, searchRequest.PageSize);
    }

    private static ExpressionStarter<Event> GetEventSearch(EventSearchRequest searchRequest)
    {
        var predicate = PredicateBuilder.New<Event>(true);

        if (!string.IsNullOrEmpty(searchRequest.SearchText))
        {
            predicate = predicate.And(e => e.Name.ToLower().Contains(searchRequest.SearchText.ToLower()));
        }
        if (!string.IsNullOrEmpty(searchRequest.Location))
        {
            predicate = predicate.And(e => e.Location!.ToLower().Contains(searchRequest.Location.ToLower()));
        }

        return predicate;
    }

    private static ExpressionStarter<Event> GetFilterQuery(EventSearchRequest searchRequest)
    {
        var predicate = PredicateBuilder.New<Event>(false);

        switch (searchRequest.SearchViewType)
        {
            case Enums.EventSearchViewType.Past:
                predicate = predicate.And(e => e.StartDate <= DateTime.UtcNow);
                break;
            default:
                predicate = predicate.And(e => e.StartDate >= DateTime.UtcNow);
                break;
        }

        if (searchRequest.Statuses != null && searchRequest.Statuses.Any())
        {
            var predicateStatuses = PredicateBuilder.New<Event>(false);

            foreach (var status in searchRequest.Statuses)
            {
                switch (status)
                {
                    case Enums.EventSearchStatus.OnSale:
                        predicateStatuses = predicateStatuses.Or(e => e.StartDate <= DateTime.UtcNow && e.TicketTypes != null &&
                                                                      e.TicketTypes.Sum(t => t.TicketsSold) != e.TicketTypes.Sum(t => t.MaxTicketsAvailable));
                        break;
                    case Enums.EventSearchStatus.SoldOut:
                        predicateStatuses = predicateStatuses.Or(e => e.StartDate <= DateTime.UtcNow && e.TicketTypes != null &&
                                                                      e.TicketTypes.Sum(t => t.TicketsSold) >= e.TicketTypes.Sum(t => t.MaxTicketsAvailable));
                        break;
                    case Enums.EventSearchStatus.Scheduled:
                        predicateStatuses = predicateStatuses.Or(e => e.StartDate >= DateTime.UtcNow && e.TicketTypes != null &&
                                                                      e.TicketTypes.Sum(t => t.TicketsSold) != e.TicketTypes.Sum(t => t.MaxTicketsAvailable));
                        break;
                    case Enums.EventSearchStatus.Draft:
                        predicateStatuses = predicateStatuses.Or(e => e.Publish == false);
                        break;
                    case Enums.EventSearchStatus.Publish:
                        predicateStatuses = predicateStatuses.Or(e => e.Publish == true);
                        break;
                }
            }

            predicate = predicate.And(predicateStatuses);
        }

        return predicate;
    }

    private IQueryable<Event> GetSortQuery(EventSearchRequest searchRequest)
    {
        IQueryable<Event> events;

        switch (searchRequest.SortColumn)
        {
            case Enums.EventSearchSortColumn.StartDateAsc:
                events = _dbContext.Events.AsQueryable().OrderBy(e => e.StartDate);
                break;
            case Enums.EventSearchSortColumn.StartDateDesc:
                events = _dbContext.Events.AsQueryable().OrderByDescending(e => e.StartDate);
                break;
            case Enums.EventSearchSortColumn.EndDateAsc:
                events = _dbContext.Events.AsQueryable().OrderBy(e => e.EndDate);
                break;
            case Enums.EventSearchSortColumn.EndDateDesc:
                events = _dbContext.Events.AsQueryable().OrderByDescending(e => e.EndDate);
                break;
            case Enums.EventSearchSortColumn.CreatedAtAsc:
                events = _dbContext.Events.AsQueryable().OrderBy(e => e.CreatedAt);
                break;
            case Enums.EventSearchSortColumn.CreatedAtDesc:
                events = _dbContext.Events.AsQueryable().OrderByDescending(e => e.CreatedAt);
                break;
            case Enums.EventSearchSortColumn.EventNameAsc:
                events = _dbContext.Events.AsQueryable().OrderBy(e => e.Name);
                break;
            case Enums.EventSearchSortColumn.EventNameDesc:
                events = _dbContext.Events.AsQueryable().OrderByDescending(e => e.Name);
                break;
            default:
                events = searchRequest.SearchViewType == Enums.EventSearchViewType.Upcoming
                    ? _dbContext.Events.AsQueryable().OrderBy(e => e.StartDate)
                    : _dbContext.Events.AsQueryable().OrderByDescending(e => e.StartDate);
                break;
        }

        return events;
    }


    public async Task<PagedResultDto<EventBasicBusinessDto>> GetEventsByChannelAsBusinessDtos(Guid channelId, int page, int pageSize)
    {
        return await GetEventBase()
            .Where(e => e.Channels.Any(c => c.Id == channelId))
            .AsSplitQuery()
            .Select(e => new EventBasicBusinessDto(e))
            .ToPagedResult(page, pageSize);
    }

    public async Task<PagedResultDto<EventDetailedBusinessDto>> GetEventsAsAdminDtos(int page,
        int pageSize)
    {
        var pages = await GetEventBase()
            .OrderByDescending(e => e.StartDate)
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);

        foreach (var @event in pages.List)
        {
            if (@event.TicketTypes != null)
            {
                foreach (var ticketType in @event.TicketTypes)
                {
                    ticketType.Revenue = await _orderAggregatorService.GetOrdersTicketTypeRevenueAsDto(ticketType.Id);
                }
            }
        }

        var dtos = await pages.List
            .SelectAsync(async e => new EventDetailedBusinessDto(e, await _orderAggregatorService.GetOrdersEventRevenueAsDto(e.Id)));
        var new_pages = new PagedResultDto<EventDetailedBusinessDto>
        {
            List = dtos.ToList(),
            CurrentPage = pages.CurrentPage,
            PageSize = pages.PageSize,
            RowCount = pages.RowCount,
            PageCount = pages.PageCount,
        };
        return new_pages;
    }

    public async Task<EventDetailedBusinessDto> GetEventAsBusinessDto(int id)
    {
        var eventEntity = await GetEventBase()
            .AsSplitQuery()
            .FirstOrThrowAsync(e => e.Id == id);
        if (eventEntity.TicketTypes != null)
        {
            foreach (var ticketType in eventEntity.TicketTypes)
            {
                ticketType.Revenue = await _orderAggregatorService.GetOrdersTicketTypeRevenueAsDto(ticketType.Id);
            }
        }
        var revenue = await _orderAggregatorService.GetOrdersEventRevenueAsDto(id);
        return new EventDetailedBusinessDto(eventEntity, revenue);
    }

    public async Task<EventDetailedBusinessDto> GetEventAsBusinessDtoByCode(string code)
    {
        var eventEntity = await GetEventBase()
            .AsSplitQuery()
            .FirstOrThrowAsync(e => e.Code == code);
        if (eventEntity.TicketTypes != null)
        {
            foreach (var ticketType in eventEntity.TicketTypes)
            {
                ticketType.Revenue = await _orderAggregatorService.GetOrdersTicketTypeRevenueAsDto(ticketType.Id);
            }
        }
        var revenue = await _orderAggregatorService.GetOrdersEventRevenueAsDto(eventEntity.Id);
        return new EventDetailedBusinessDto(eventEntity, revenue);
    }

    public async Task<EventTicketStatsBusinessDto> GetEventTicketStatsAsBusinessDto(int id)
    {
        var eventEntity = await GetEventBase()
            .FirstOrThrowAsync(e => e.Id == id);
        var tickets = await _dbContext.Tickets
            .Include(t => t.TicketType!.Event)
            .Include(t => t.Owner)
            .Where(t => t.TicketType != null && eventEntity.TicketTypes!.Contains(t.TicketType)) // not very fast
            .ToListAsync();
        return new EventTicketStatsBusinessDto(eventEntity, tickets);
    }
    public async Task<Result<List<TicketTypeDashboardDto>>> GetEventTicketTypes(int id) =>
        await _dbContext.EventTicketTypes
            .Include(e => e.Image)
            .Where(e => e.EventId == id)
            .OrderBy(e => e.Name)
            .Select(e => new TicketTypeDashboardDto(e))
            .ToListAsync();

    public async Task ArrangeTicketsDto(EventArrangeTicketsBusinessDto[] dto)
    {
        var map = dto.ToDictionary(x => x.TicketId, x => x.Index);
        await _dbContext.EventTicketTypes
            .Where(t => map.Keys.Contains(t.Id))
            .ForEachAsync(t => t.SortIndex = map[t.Id]);
        await _dbContext.SaveChangesAsync();
    }

    private async Task SetChannelsForEvent(Event eventEntity, string channelIdsCsv)
    {
        var channelIds = new List<Guid>();
        try
        {
            channelIds = channelIdsCsv.Split(',').Select(Guid.Parse).ToList();
        }
        catch (Exception)
        {
            throw new BadRequestException($"ChannelIdsCsv {channelIdsCsv} contains invalid guids");
        }
        if (channelIds.Count == 0)
            throw new BadRequestException("Event must have at least one channel");
        eventEntity.Channels.Clear();
        foreach (var channelId in channelIds)
        {
            var channel = await _channelService.GetChannel(channelId);
            eventEntity.Channels.Add(channel);
        }
    }

    public async Task<EventDetailedBusinessDto> CreateEvent(EventCreateBusinessDto dto, string creatorId)
    {
        if (dto.CustomSlug != null)
            await VerifySlug(dto.CustomSlug);
        if (dto.ExternalEventUrl != null)
            VerifyUrl(dto.ExternalEventUrl);

        var businessId = await _userService.GetUserBusinessId(creatorId);

        // Handle the new image if provided in the DTO, and return the new image's DbModel if created
        var dbImage = await _imageStorageService.UploadFile(dto.Image);
        var dbTicketImage = await _imageStorageService.UploadFile(dto.TicketBackgroundImage);

        var eventEntity = new Event();
        _dbContext.Entry(eventEntity).CurrentValues.SetValues(dto);
        eventEntity.BusinessId = businessId;
        eventEntity.CreatorId = creatorId;
        eventEntity.Image = dbImage;
        eventEntity.TicketBackgroundImage = dbTicketImage;

        await SetChannelsForEvent(eventEntity, dto.ChannelIdsCsv);

        if (eventEntity.Channels.Count == 0)
        {
            var channel = await _channelService.GetOrCreateDefaultChannel(businessId);
            eventEntity.Channels.Add(channel);
        }
        _dbContext.Events.Add(eventEntity);

        // Save changes to the database, and delete the new image if the save operation fails
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            if (dbImage != null)
                await _imageStorageService.DeleteFile(dbImage.StorageIdentifier);
            if (dbTicketImage != null)
                await _imageStorageService.DeleteFile(dbTicketImage.StorageIdentifier);
            throw;
        }

        _logger.LogInformation("New event {EventId} created by user: {CreatorId}", eventEntity.Id, creatorId);

        return await GetEventAsBusinessDto(eventEntity.Id);
    }

    public async Task<Result<int>> CreateEventRequest(EventCreateRequest createRequest, string creatorId)
    {
        var validationResult = await _eventCreateRequestValidator.ValidateAsync(createRequest);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.ToErrorMessage());
        }

        ImageDbModel? dbImage = null;
        try
        {
            dbImage = await _imageStorageService.UploadFile(createRequest.Image);
            var businessId = await _userService.GetUserBusinessId(creatorId);

            var eventEntity = new Event();
            _dbContext.Entry(eventEntity).CurrentValues.SetValues(createRequest);
            eventEntity.BusinessId = businessId;
            eventEntity.CreatorId = creatorId;
            eventEntity.Image = dbImage;

            _dbContext.Events.Add(eventEntity);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("New event {EventId} created by user: {CreatorId}", eventEntity.Id, creatorId);

            return eventEntity.Id;
        }
        catch
        {
            if (dbImage != null)
                await _imageStorageService.DeleteFile(dbImage.StorageIdentifier);
            throw;
        }
    }

    public async Task<Result<EventBasicBusinessDto>> UpdateEventRequest(EventUpdateRequest updateRequest)
    {
        var validationResult = await _eventUpdateRequestValidator.ValidateAsync(updateRequest);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.ToErrorMessage());
        }

        var eventEntity = await _dbContext.Events
            .Include(e => e.Image)
            .Include(e => e.TicketBackgroundImage)
            .FirstOrThrowAsync(e => e.Id == updateRequest.Id);

        // Handle the new image if provided in the DTO, and return the new image's DbModel if created
        var newDbImage = await _imageStorageService.UploadFile(updateRequest.Image);

        // Store the old image ID for later deletion if a new image is provided or the DTO specifies the image should be deleted
        Guid? oldImageIdToDelete = null;
        if (eventEntity.Image != null && newDbImage != null)
        {
            oldImageIdToDelete = eventEntity.Image.StorageIdentifier;
            _dbContext.Remove(eventEntity.Image);
        }

        _dbContext.Entry(eventEntity).CurrentValues.SetValues(updateRequest);

        // Update the image only if a new one was uploaded or if the old one was deleted
        if (newDbImage != null)
        {
            eventEntity.Image = newDbImage;
        }

        // Save changes to the database, and delete the new image if the save operation fails
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            if (newDbImage != null)
                await _imageStorageService.DeleteFile(newDbImage.StorageIdentifier);
            throw;
        }

        // If a new image was uploaded successfully or the old one was deleted, delete the old image file from storage
        if (oldImageIdToDelete != null)
            await _imageStorageService.DeleteFile(oldImageIdToDelete.Value);

        _logger.LogInformation("Event {EventId} updated", eventEntity.Id);

        return new EventBasicBusinessDto(eventEntity);
    }
    public async Task<Event> GetEvent(int id)
    {
        return await _dbContext.Events
            .FirstOrThrowAsync(ts => ts.Id == id);
    }

    public async Task<Result<EventBasicBusinessDto>> GetEventDto(int id)
    {
        return new EventBasicBusinessDto(await GetEvent(id));
    }

    public async Task<Result<string>> PublishEvent(int id, bool publish)
    {
        var eventEntity = await _dbContext.Events
                                .FirstOrThrowAsync(e => e.Id == id);

        eventEntity.Publish = publish;
        _dbContext.Events.Update(eventEntity);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Publish event for event id : {eventEntity.Id}");

        return "Successfully publish event";
    }

    public async Task<EventDetailedBusinessDto> UpdateEvent(int id, EventUpdateBusinessDto dto)
    {
        var eventEntity = await _dbContext.Events
            .Include(e => e.Image)
            .Include(e => e.TicketBackgroundImage)
            .Include(e => e.Channels)
            .FirstOrThrowAsync(e => e.Id == id);

        if (dto.CustomSlug != null)
            await VerifySlug(dto.CustomSlug, eventEntity);
        if (dto.ExternalEventUrl != null)
            VerifyUrl(dto.ExternalEventUrl);

        // Handle the new image if provided in the DTO, and return the new image's DbModel if created
        var newDbImage = await _imageStorageService.UploadFile(dto.Image);
        var newDbTicketImage = await _imageStorageService.UploadFile(dto.TicketBackgroundImage);

        // Store the old image ID for later deletion if a new image is provided or the DTO specifies the image should be deleted
        Guid? oldImageIdToDelete = null;
        if (eventEntity.Image != null && (newDbImage != null || dto.DeleteImage))
        {
            oldImageIdToDelete = eventEntity.Image.StorageIdentifier;
            _dbContext.Remove(eventEntity.Image);
        }

        Guid? oldTicketImageIdToDelete = null;
        if (eventEntity.TicketBackgroundImage != null && (newDbTicketImage != null || dto.DeleteTicketBackgroundImage))
        {
            oldTicketImageIdToDelete = eventEntity.TicketBackgroundImage.StorageIdentifier;
            _dbContext.Remove(eventEntity.TicketBackgroundImage);
        }

        if (dto.ChannelIdsCsv != null)
            await SetChannelsForEvent(eventEntity, dto.ChannelIdsCsv);

        _dbContext.Entry(eventEntity).CurrentValues.SetValues(dto);

        // Update the image only if a new one was uploaded or if the old one was deleted
        if (newDbImage != null || dto.DeleteImage)
        {
            eventEntity.Image = newDbImage;
        }

        if (newDbTicketImage != null || dto.DeleteTicketBackgroundImage)
        {
            eventEntity.TicketBackgroundImage = newDbTicketImage;
        }

        // Save changes to the database, and delete the new image if the save operation fails
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            if (newDbImage != null)
                await _imageStorageService.DeleteFile(newDbImage.StorageIdentifier);
            if (newDbTicketImage != null)
                await _imageStorageService.DeleteFile(newDbTicketImage.StorageIdentifier);
            throw;
        }


        // If a new image was uploaded successfully or the old one was deleted, delete the old image file from storage
        if (oldImageIdToDelete != null)
            await _imageStorageService.DeleteFile(oldImageIdToDelete.Value);

        if (oldTicketImageIdToDelete != null)
            await _imageStorageService.DeleteFile(oldTicketImageIdToDelete.Value);

        _logger.LogInformation("Event {EventId} updated", eventEntity.Id);

        return await GetEventAsBusinessDto(eventEntity.Id);
    }

    public async Task VerifySlug(string slug, Event? dbEvent = null)
    {
        RandomStringsHelper.VerifySlug(slug);

        var slugExists = await _dbContext.Events.AnyAsync(e => e.CustomSlug == slug && e != dbEvent);
        if (slugExists)
            throw new SlugAlreadyUsedException("Slug is already in use");
    }

    public void VerifyUrl(string url)
    {
        Uri? uri;

        if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            throw new InvalidUrlException($"Url \"{url}\" is invalid");
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new InvalidUrlException($"Url \"{url}\" is invalid");
    }

    public async Task<EventDetailedBusinessDto> MakeEventVisible(int id)
    {
        var eventEntity = await _dbContext.Events.FirstOrThrowAsync(e => e.Id == id);
        eventEntity.Visible = true;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Event {EventId} made visible", eventEntity.Id);

        return await GetEventAsBusinessDto(eventEntity.Id);
    }

    public async Task<EventDetailedBusinessDto> HideEvent(int id)
    {
        var eventEntity = await _dbContext.Events.FirstOrThrowAsync(e => e.Id == id);
        eventEntity.Visible = false;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Event {EventId} made hidden", eventEntity.Id);

        return await GetEventAsBusinessDto(eventEntity.Id);
    }

    public async Task<EventDetailedBusinessDto> GenerateEventCode(int id)
    {
        var eventEntity = await _dbContext.Events.FirstOrThrowAsync(e => e.Id == id);
        if (eventEntity.Code != null)
        {
            throw new BadRequestException("Event already has a view code, if a new one is needed, an existing code needs to be removed first");
        }
        eventEntity.Code = RandomStringsHelper.RandomString(EventViewCodeLength);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Event {eventEntity.Id} code {eventEntity.Code} generated");

        return await GetEventAsBusinessDto(eventEntity.Id);
    }

    public async Task<EventDetailedBusinessDto> RemoveEventCode(int id)
    {
        var eventEntity = await _dbContext.Events.FirstOrThrowAsync(e => e.Id == id);
        var code = eventEntity.Code;
        eventEntity.Code = null;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"Event {eventEntity.Id} code removed was {code}");

        return await GetEventAsBusinessDto(eventEntity.Id);
    }

    public async Task<EventTicketTypeBusinessDto> CreateEventTicketType(int eventId, EventTicketTypeUpdateBusinessDto dto)
    {
        ValidateTicketDto(dto);
        var eventTicketType = new EventTicketType();
        _dbContext.Add(eventTicketType);
        eventTicketType.EventId = eventId;
        _dbContext.Entry(eventTicketType).CurrentValues.SetValues(dto);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Ticket type for event {EventId} with id {TicketTypeId} was created", eventId, eventTicketType.Id);
        return new EventTicketTypeBusinessDto(eventTicketType);
    }

    public async Task<EventTicketTypeBusinessDto> UpdateEventTicketType(Guid id, EventTicketTypeUpdateBusinessDto dto)
    {
        ValidateTicketDto(dto);
        var eventTicketType = await _dbContext.EventTicketTypes.FirstOrThrowAsync(e => e.Id == id);
        _dbContext.Entry(eventTicketType).CurrentValues.SetValues(dto);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Ticket type with id {TicketTypeId} was updated", id);
        return new EventTicketTypeBusinessDto(eventTicketType);
    }

    private void ValidateTicketDto(EventTicketTypeUpdateBusinessDto dto)
    {
        if (dto.MinTicketsPerPurchase > dto.MaxTicketsPerPurchase)
            throw new BadRequestException($"Minimum tickets per purchase {dto.MinTicketsPerPurchase} cannot be greater than maximum tickets per purchase {dto.MaxTicketsPerPurchase}");
        if (dto.MinTicketsPerPurchase > dto.MaxTicketsAvailable)
            throw new BadRequestException($"Minimum tickets per purchase {dto.MinTicketsPerPurchase} cannot be greater than maximum tickets available {dto.MaxTicketsAvailable}");
        if (dto.MaxTicketsPerPurchase > dto.MaxTicketsAvailable)
            throw new BadRequestException($"Maximum tickets per purchase {dto.MaxTicketsPerPurchase} cannot be greater than maximum tickets available {dto.MaxTicketsAvailable}");
    }

    public async Task<Result<IReadOnlyList<ChannelEventBusinessDto>>> UpdateChannelResaleOptions(
        int eventId, ChannelEventUpdateResaleRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _channelEventUpdateResaleRequestValidator
            .ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.ToErrorMessage());
        }

        var channelEvents = await _dbContext
            .ChannelEvents
            .Where(@event => @event.EventId == eventId
                             && request.ChannelUpdateResaleRequests
                                 .Select(r => r.ChannelId)
                                 .Contains(@event.ChannelId))
            .ToListAsync(cancellationToken);

        if (channelEvents.Count != request.ChannelUpdateResaleRequests.Count)
        {
            return Result.Fail(
                new CelebratixError(
                    ErrorCode.CelebratixChannelInvalidOrNotFound,
                    "Only channels for the specified event are allowed."));
        }

        foreach (var channelEvent in channelEvents)
        {
            var update = request.ChannelUpdateResaleRequests
                .Single(r => r.ChannelId == channelEvent.ChannelId);

            channelEvent.ResaleEnabled = update.Enabled;

            channelEvent.ResaleDisabledDescription = update.Enabled
                ? null
                : update.DisabledDescription;

            channelEvent.ResaleRedirectUrl =
                !update.Enabled
                || string.IsNullOrWhiteSpace(update.RedirectUrl)
                    ? null
                    : new Uri(update.RedirectUrl);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return channelEvents
            .Where(c => request.ChannelUpdateResaleRequests.Any(r => r.ChannelId == c.ChannelId))
            .Select(@event => new ChannelEventBusinessDto(@event))
            .ToArray();
    }

    public async Task<Result<EventBasicBusinessDto>> Publish(int eventId, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.Events
            .Include(e => e.Channels)
            .Include(e => e.TicketTypes)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);

        if (@event == null)
        {
            return Result.Fail(
                new CelebratixError(
                    ErrorCode.CelebratixEventInvalidOrNotFound,
                    $"No event was found with id '{eventId}'."));
        }

        if (@event.TicketTypes == null)
        {
            return Result.Fail(
                new CelebratixError(
                    ErrorCode.CelebratixEventNoTicketTypesFound,
                    $"The event with id '{eventId}' is missing at least one 'TicketType'."));
        }

        if (!@event.Channels.Any())
        {
            return Result.Fail(
                new CelebratixError(
                    ErrorCode.CelebratixEventNoChannelsFound,
                    $"The event with id '{eventId}' is missing at least one 'Channel'."));
        }

        @event.Publish = true;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new EventBasicBusinessDto(@event);

    }
}
