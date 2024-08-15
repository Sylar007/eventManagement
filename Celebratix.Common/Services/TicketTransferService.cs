using System.Data;
using Celebratix.Common.Database;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User.Tickets;
using Celebratix.Common.Models.DTOs.User.TicketTransfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class TicketTransferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TicketTransferService> _logger;

    private const int TicketTransferOfferCodeLength = 12;

    public enum CancelReason
    {
        UserAction,
        EventEnded,
        TicketScanned
    }

    public TicketTransferService(ILogger<TicketTransferService> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResultDto<TicketTransferOfferDto>> GetAcceptedTransferOffers(string userId, int page, int pageSize)
    {
        var query = _unitOfWork.DbContext.TicketTransferOffers
            .Include(to => to.Ticket!.TicketType!.Event!.Category)
            .Include(to => to.Ticket!.TicketType!.Event!.Image)
            .Include(to => to.FulfilledByOrder)
            .Where(to => to.ReceiverId == userId)
            .Where(to => !to.Cancelled);

        var transferOffers = await query
            .Select(to => new TicketTransferOfferDto(to))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);

        return transferOffers;
    }

    public async Task<PagedResultDto<TicketTransferOfferDto>> GetOutgoingTransferOffers(string userId, bool onlyActive, int page, int pageSize)
    {
        var query = _unitOfWork.DbContext.TicketTransferOffers
            .Include(to => to.Ticket!.TicketType!.Event!.Category)
            .Include(to => to.Ticket!.TicketType!.Event!.Image)
            .Include(to => to.FulfilledByOrder)
            .Where(to => to.TransferorId == userId)
            .Where(to => !to.Cancelled);

        if (onlyActive)
        {
            query = query
                .Where(to => to.TransferredAt == null)
                .OrderByDescending(to => to.CreatedAt);
        }
        else
        {
            query = query
                .OrderByDescending(to => to.CreatedAt)
                .ThenByDescending(to => to.TransferredAt == null);
        }

        var transferOffers = await query
            .Select(to => new TicketTransferOfferDto(to))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);

        return transferOffers;
    }

    public async Task<TicketTransferOfferDto> GetTransferOfferAsUserDto(Guid id)
    {
        var offer = await _unitOfWork.DbContext.TicketTransferOffers
            .Include(to => to.Ticket!.TicketType!.Event!.Category)
            .Include(to => to.Ticket!.TicketType!.Event!.Image)
            .Include(to => to.FulfilledByOrder!.TicketType!.Event)
            .FirstOrThrowAsync(t => t.Id == id);
        return new TicketTransferOfferDto(offer);
    }

    public async Task<TicketTransferOfferDto> CreateTransferOffer(string userId, Guid ticketId)
    {
        await using var transaction = await _unitOfWork.CreateTransactionAsync();

        var ticket = await _unitOfWork.DbContext.Tickets
            .Where(t => t.OwnerId == userId)
            .Include(t => t.TicketType!.Event)
            .FirstOrThrowAsync(t => t.Id == ticketId);

        var action = new TicketQrActions(ticket, ticket.OwnerId!, "");
        switch (action.Transfer.UnavailableReason)
        {
            case null:
                break;
            case TransferUnavailableReason.CheckedIn:
                throw new TicketAlreadyScannedException();
            case TransferUnavailableReason.MarketplaceListing:
                throw new TicketInMarketplaceListingException();
            case TransferUnavailableReason.SoldOrTransferred:
                throw new TicketSoldOrTransferredException();
            case TransferUnavailableReason.EventPassed:
                throw new EventEndTimePassedException();
            default:
                throw new Exception($"Unknown {nameof(TransferUnavailableReason)}: {action.Transfer.UnavailableReason}");
        };

        if (ticket.ActiveTicketTransferOfferId != null)
            throw new TicketInTransferOfferException();

        var transferOffer = new TicketTransferOffer
        {
            Ticket = ticket,
            TransferorId = userId,
            Code = RandomStringsHelper.RandomString(TicketTransferOfferCodeLength)
        };

        ticket.ActiveTicketTransferOffer = transferOffer;
        _unitOfWork.DbContext.TicketTransferOffers.Add(transferOffer);

        await transaction.CompleteAsync();

        _logger.LogInformation($"Created a new {nameof(TicketTransferOffer)}\n" +
                               $"  Id               : {transferOffer.Id}\n" +
                               $"  TransferorId     : {transferOffer.TransferorId}"
        );
        return await GetTransferOfferAsUserDto(transferOffer.Id);
    }

    public async Task<TicketTransferOfferDto> GetTransferOfferByIdForUser(string userId, Guid id)
    {
        var offer = await _unitOfWork.DbContext.TicketTransferOffers
            .Include(to => to.Ticket)
            .Include(to => to.Ticket!.TicketType!.Event!.Category)
            .Include(to => to.Ticket!.TicketType!.Event!.Image)
            .Include(to => to.FulfilledByOrder)
            .Where(to => to.Id == id && to.TransferorId == userId)
            .FirstOrThrowAsync();

        return new TicketTransferOfferDto(offer);
    }

    public async Task<TicketTransferOfferDto> GetTransferOfferByCode(string code)
    {
        var transferOffer = await _unitOfWork.DbContext.TicketTransferOffers
        .Include(to => to.Ticket)
        .Where(to => to.Code == code)
        .FirstOrThrowAsync();

        return await GetTransferOfferAsUserDto(transferOffer.Id);
    }

    // TODO: Create order etc.?

    public async Task<TicketTransferOfferDto> AcceptTransferOffer(string userId, string code)
    {
        await using var transaction = await _unitOfWork.CreateTransactionAsync(IsolationLevel.RepeatableRead);

        var transferOffer = await _unitOfWork.DbContext.TicketTransferOffers
            .Include(to => to.Ticket)
                .ThenInclude(t => t!.TicketType!.Event)
            .Where(to => to.Code == code)
            .FirstOrThrowAsync();

        if (transferOffer.Available && transferOffer.TransferorId != userId)
        {
            transferOffer.ReceiverId = userId;
            transferOffer.TransferredAt = DateTimeOffset.UtcNow;
            transferOffer.Ticket!.OwnerId = userId;
            transferOffer.Ticket!.ActiveTicketTransferOfferId = null;

            // Create the Order
            var order = new Order
            {
                Status = Enums.OrderStatus.Completed,
                CompletedAt = DateTimeOffset.UtcNow,
                PurchaserId = userId,
                TicketTypeId = transferOffer.Ticket!.TicketTypeId,
                TicketType = transferOffer.Ticket!.TicketType,
                TicketTransferOfferId = transferOffer.Id,
                TicketQuantity = 1,
                EventId = transferOffer.Ticket!.TicketType!.EventId,
                BaseAmount = 0m, // Free transfer
                ServiceAmount = 0m,
                ApplicationAmount = 0m,
                Vat = 0m, // No VAT on free transfer
                CurrencyId = null,
                Tickets = new List<Ticket> { transferOffer.Ticket }
            };
            transferOffer.Ticket!.LatestOrder = order;

            transferOffer.FulfilledByOrder = order;

        }
        else
        {
            throw new TransferOfferNotAvailableException("Transfer offer is not available to accept or the user is the same as the transferor");
        }
        await transaction.CompleteAsync();
        _logger.LogInformation($"Accepted {nameof(TicketTransferOffer)}" +
                                 $"  Id       : {transferOffer.Id}\n" +
                                 $"  From     : {transferOffer.TransferorId}\n" +
                                 $"  To       : {transferOffer.ReceiverId}");
        return await GetTransferOfferAsUserDto(transferOffer.Id);
    }

    public void CancelTransferOffer(TicketTransferOffer transferOffer, CancelReason reason)
    {
        if (transferOffer.Cancelled == true && transferOffer.Ticket!.ActiveTicketTransferOfferId == null)
            return;

        transferOffer.Cancelled = true;
        transferOffer.Ticket!.ActiveTicketTransferOfferId = null;

        _logger.LogInformation(
            $"Cancelling {nameof(TicketTransferOffer)}:\n" +
            $"  Reason : {reason}\n" +
            $"  Id     : {transferOffer.Id}\n" +
            $"  From   : {transferOffer.TransferorId}\n" +
            $"  To     : {transferOffer.ReceiverId}\n" +
            $"  Ticket : {transferOffer.TicketId}\n" +
            $"  Code   : {transferOffer.Code}"
            );
    }

    public async Task CancelTransferOffersForTicket(Guid ticketId, CancelReason reason)
    {
        await _unitOfWork.DbContext.TicketTransferOffers
            .Include(to => to.Ticket)
            .Where(to => to.TicketId == ticketId)
            .Where(to => !to.Cancelled)
            .ForEachAsync(to => CancelTransferOffer(to, reason));
    }

    public async Task<TicketTransferOfferDto> CancelTransferOffer(string userId, Guid id, CancelReason reason)
    {
        var transferOffer = await _unitOfWork.DbContext.TicketTransferOffers
            .Include(to => to.Ticket)
            .Where(to => to.TransferorId == userId)
            .FirstOrThrowAsync(to => to.Id == id);

        if (transferOffer.Available)
        {
            CancelTransferOffer(transferOffer, reason);
            await _unitOfWork.DbContext.SaveChangesAsync();
        }
        else
        {
            throw new TransferOfferNotAvailableException("Transfer offer is not available to cancel");
        }

        return await GetTransferOfferAsUserDto(transferOffer.Id);
    }

    public async Task CancelOffersForPastEvents()
    {
        var counter = 0;
        var currentDatetime = DateTimeOffset.UtcNow;

        // Putting all of this in a RepeatableRead transaction is potentially problematic for performance in scenarios
        // with hundreds or thousands of offers that are to be cancelled.
        // It can lead to locking for a long time
        // Maybe add "max 1000" and run it every minute or something?
        await using var transaction = await _unitOfWork.CreateTransactionAsync(IsolationLevel.RepeatableRead);

        var offers = _unitOfWork.DbContext.TicketTransferOffers
            .Include(ml => ml.Ticket)
            .Where(ml => ml.Cancelled == false)
            .Where(ml => ml.TransferredAt == null)
            .Where(ml => currentDatetime > ml.Ticket!.TicketType!.Event!.EndDate)
            .AsAsyncEnumerable();

        await foreach (var listing in offers)
        {
            CancelTransferOffer(listing, CancelReason.EventEnded);
            counter++;
        }

        await transaction.CompleteAsync();
        if (counter > 0)
            _logger.LogInformation("Finished processing cancelling of transfer offers of past events. Total number {Number}", counter);
    }
}
