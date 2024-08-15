using System.Data;
using Celebratix.Common.Configs;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Celebratix.Common.Services;

public class OrderService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly MarketplaceConfig _marketplaceConfig;
    private readonly PaymentService _paymentService;
    private readonly AffiliateCodeService _affiliateCodeService;
    private readonly ILogger<OrderService> _logger;

    private readonly int _orderTimeoutMinutes;

    public OrderService(CelebratixDbContext dbContext, ILogger<OrderService> logger, PaymentService paymentService,
        IOptions<MarketplaceConfig> marketplaceConfig, AffiliateCodeService affiliateCodeService, IOptions<StripeConfig> stripeConfig)
    {
        _dbContext = dbContext;
        _logger = logger;
        _paymentService = paymentService;
        _marketplaceConfig = marketplaceConfig.Value;
        _affiliateCodeService = affiliateCodeService;
        _orderTimeoutMinutes = stripeConfig.Value.OrderTimeoutMinutes;
    }

    public async Task<OrderCreateResponseDto> PlacePrimaryMarketOrder(string userId, PrimaryMarketOrderCreateDto dto)
    {
        _logger.LogInformation($"Creating an Order for user: {userId}, TicketType: {dto.TicketTypeId}, Quantity: {dto.TicketQuantity}, AffiliateCode: {dto.AffiliateCode}");
        Event dbEvent;
        try
        {
            dbEvent = await GetEventWithIncludesFromTicketTypeId(dto.TicketTypeId);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Failed to get an Event from TicketType: {dto.TicketTypeId} with Exception: {e}");
            throw;
        }
        Affiliate? affiliateCode = null;
        if (dto.AffiliateCode != null)
        {
            try
            {
                affiliateCode = await _affiliateCodeService.GetAffiliateCodeByCode(dto.AffiliateCode);
            }
            catch (Exception e)
            {
                // It could be that the user mistyped the affiliate code, so we don't want to throw an exception here
                _logger.LogWarning($"Failed to validate AffiliateCode: {dto.AffiliateCode} with Exception: {e}");
            }
        }

        var business = dbEvent.Business!;
        var currency = dbEvent.Currency!;

        string stripeCustomerId;
        try
        {
            stripeCustomerId = await _paymentService.GetOrCreateStripeCustomerId(userId);
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Failed to get or create stripeCustomerId for user: {userId} with Exception: {e}");
            throw;
        }

        var order = CreateOrderModel(userId, dto.TicketTypeId, dto.TicketQuantity, currency.Code, null, affiliateCode?.Id);
        var responseDto = new OrderCreateResponseDto
        {
            StripeCustomerId = stripeCustomerId
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        try
        {
            var ticketType = await _dbContext.EventTicketTypes
                .FirstOrThrowAsync(tt => tt.Id == dto.TicketTypeId);

            ValidateTicketQuantity(dto.TicketQuantity, ticketType);

            ticketType.ReservedTickets += dto.TicketQuantity;
            order.TicketType = ticketType;
            order.EventId = ticketType.EventId;
            var amount = ticketType.TotalPrice * dto.TicketQuantity;

            if (amount == decimal.Zero)
            {
                order.BaseAmount = decimal.Zero;
                order.ServiceAmount = decimal.Zero;
                order.ApplicationAmount = decimal.Zero;
                order.Vat = decimal.Zero;
                UpdateStatusAndFulfillOrder(order);
            }
            else
            {
                // To increase performance these stripe requests could be moved outside of the transaction.
                // Before for simplicity (but potentially leading to intents being created while the transaction fails)
                // Or after, could lead to Orders without valid stripe intents (which would be cleaned up by hangfire,
                // can even clean them up without waiting for the time limit).
                // But would have to save changes to the db once more to set the IntentId on the order after the transaction
                var ephemeralKey = await _paymentService.GetEphemeralKeyForStripeCustomer(stripeCustomerId);
                var paymentIntent = await _paymentService.CreateStripePaymentIntent(stripeCustomerId, amount, currency);

                order.BaseAmount = ticketType.Price * dto.TicketQuantity;
                order.ServiceAmount = ticketType.ServiceFee * dto.TicketQuantity;
                order.ApplicationAmount = ticketType.ApplicationFee * dto.TicketQuantity;
                order.Vat = ticketType.CustomVat;
                order.StripePaymentIntentId = paymentIntent.Id;

                responseDto.PaymentIntentClientSecret = paymentIntent.ClientSecret;
                responseDto.EphemeralKeySecret = ephemeralKey.Secret;
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Order needs to have id set before sending emails
            if (order.Status == Enums.OrderStatus.Completed)
            {
                await SendOrderEmails(order, userId, dbEvent);
            }

            responseDto.OrderId = order.Id;
            responseDto.OrderStatus = order.Status;
            responseDto.ValidUntil = order.CreatedAt.AddMinutes(_orderTimeoutMinutes);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                "Error (potential transaction error) occured when creating order for user: {UserId} for TicketType {TicketTypeId}. Error: {ErrorMessage}",
                userId, dto.TicketTypeId, e.Message);
            throw;
        }

        _logger.LogInformation("Created a new Order for user: {UserId}, TicketType: {TicketTypeId}, Quantity: {AmountOfTickets}",
            userId, dto.TicketTypeId, dto.TicketQuantity);

        return responseDto;
    }

    public async Task<OrderCreateResponseDto> PlaceMarketplaceOrder(string userId, Guid listingId)
    {
        var ticketTypeId = await _dbContext.MarketplaceListings
            .Where(ml => ml.Id == listingId)
            .Select(ml => ml.Ticket!.TicketTypeId)
            .FirstOrThrowAsync();

        var currency = await _dbContext.MarketplaceListings
            .Where(ml => ml.Id == listingId)
            .Select(ml => ml.Currency!)
            .FirstOrThrowAsync();

        var seller = await _dbContext.MarketplaceListings
            .Where(ml => ml.Id == listingId)
            .Select(ml => ml.Seller!)
            .FirstOrThrowAsync();

        var stripeCustomerId = await _paymentService.GetOrCreateStripeCustomerId(userId);

        var order = CreateOrderModel(userId, ticketTypeId, 1, currency.Code, listingId);

        var responseDto = new OrderCreateResponseDto
        {
            StripeCustomerId = stripeCustomerId
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        try
        {
            var dbListing = await _dbContext.MarketplaceListings
                .Include(ml => ml.Ticket)
                .FirstOrThrowAsync(ml => ml.Id == listingId);
            ValidateListingAvailableForOrder(dbListing);

            var ticketType = await _dbContext.EventTicketTypes
                .FirstOrThrowAsync(tt => tt.Id == ticketTypeId);

            order.EventId = ticketType.EventId;

            order.MarketplaceListing = dbListing;
            var amount = dbListing.ListingPrice * order.TicketQuantity;

            if (amount == decimal.Zero)
            {
                order.BaseAmount = decimal.Zero;
                order.ServiceAmount = decimal.Zero;
                order.ApplicationAmount = decimal.Zero;
                order.Vat = decimal.Zero;
                UpdateStatusAndFulfillOrder(order);
            }
            else
            {
                var ephemeralKey = await _paymentService.GetEphemeralKeyForStripeCustomer(stripeCustomerId);
                var paymentIntent = await _paymentService.CreateStripePaymentIntent(stripeCustomerId, amount, currency, seller.StripeConnectAccountId);

                dbListing.ReservedAt = DateTimeOffset.UtcNow;

                order.BaseAmount = amount; // TODO: this is probably incorrect, as there is a serviceFee stored, but no application fee for some reason
                order.ServiceAmount = decimal.Zero;
                order.ApplicationAmount = decimal.Zero;
                order.Vat = 0.09m;
                order.StripePaymentIntentId = paymentIntent.Id;

                responseDto.PaymentIntentClientSecret = paymentIntent.ClientSecret;
                responseDto.EphemeralKeySecret = ephemeralKey.Secret;
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Order needs to have id set before sending emails
            if (order.Status == Enums.OrderStatus.Completed)
            {
                await SendOrderEmails(order, userId);
            }

            responseDto.OrderId = order.Id;
            responseDto.OrderStatus = order.Status;
            responseDto.ValidUntil = order.CreatedAt.AddMinutes(_orderTimeoutMinutes);
        }
        catch (Exception e)
        {
            _logger.LogWarning(
                "Error (potential transaction error) occured when creating order for user: {UserId}, marketplace listing: {ListingId}. Error: {ErrorMessage}",
                userId, listingId, e.Message);
            throw;
        }

        _logger.LogInformation("Created a new Order for user: {UserId}, marketplace listing: {ListingId}",
            userId, listingId);

        return responseDto;
    }

    private static Order CreateOrderModel(string userId, Guid ticketTypeId, int ticketQuantity, string currencyCode, Guid? marketplaceListingId = null, Guid? affiliateCodeId = null)
    {
        return new Order
        {
            CurrencyId = currencyCode,
            PurchaserId = userId,
            TicketTypeId = ticketTypeId,
            TicketQuantity = ticketQuantity,
            MarketplaceListingId = marketplaceListingId,
            AffiliateCodeId = affiliateCodeId,
            Status = Enums.OrderStatus.AwaitingPaymentInfo
        };
    }

    private static void ValidateListingAvailableForOrder(MarketplaceListing listing)
    {
        if (!listing.Available)
        {
            throw new ListingNotAvailableException();
        }
    }

    private static void ValidateTicketQuantity(int ticketQuantity, EventTicketType ticketType)
    {
        var min = 1; /* Math.Min(ticketType.MinTicketsPerPurchase, ticketType.AvailableTickets ?? int.MaxValue); */ // TODO: enable again
        if (ticketQuantity < min)
            throw new AllowedTicketsLimitExceededException($"The number of requested tickets {ticketQuantity} was lower than the minimum of {min}");
        if (ticketQuantity > ticketType.MaxTicketsPerPurchase)
            throw new AllowedTicketsLimitExceededException($"The number of requested tickets {ticketQuantity} was higher than the maximum of {ticketType.MaxTicketsPerPurchase}");
        if (ticketQuantity > ticketType.AvailableTickets)
            throw new RequestedTicketsNotAvailableException($"The number of requested tickets {ticketQuantity} was higher than the available tickets {ticketType.AvailableTickets}");
    }

    private async Task<Event> GetEventWithIncludesFromTicketTypeId(Guid ticketTypeId)
    {
        return await _dbContext.EventTicketTypes
            .Where(tt => tt.Id == ticketTypeId)
            .Where(tt => tt.Event!.Visible)
            .Where(tt => tt.Event!.EndDate >= DateTimeOffset.UtcNow)
            .Include(tt => tt.Event!.Currency)
            .Include(tt => tt.Event!.Business)
            .Select(tt => tt.Event!)
            .FirstOrThrowAsync();
    }

    /// <summary>
    /// Throws ObjectNotFoundException if no cancellable order was found for the given id
    /// </summary>
    public async Task CancelOrder(string userId, Guid orderId)
    {
        // Alternative here would be to use optimistic concurrency control on TicketType
        // (don't think both would be required as the Reserved & Sold tickets would've been updated between the OrderStates)
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        var order = await _dbContext.Orders
            .Where(o => o.Status == Enums.OrderStatus.AwaitingPaymentInfo)
            .Where(o => o.PurchaserId == userId)
            .Include(o => o.TicketType)
            .Include(o => o.MarketplaceListing)
            .FirstOrThrowAsync(o => o.Id == orderId);

        await UpdateStatusAndCancelOrder(order);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation("Successfully cancelled order with id: {OrderId}", order.Id);
    }

    public async Task SetOrderToRequiresUserAction(string paymentIntentId)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var order = await _dbContext.Orders
            .FirstOrThrowAsync(o => o.StripePaymentIntentId == paymentIntentId);

        if (order.Status is not (Enums.OrderStatus.AwaitingPaymentInfo or Enums.OrderStatus.RequiresUserAction or Enums.OrderStatus.Processing))
        {
            _logger.LogCritical("Order is in status {OrderStatus}, an illegal status for setting the order to requires user action", order.Status);
            throw new BadOrderStatusException(
                $"Order is in status {order.Status}, an illegal status for setting the order to requires user action");
        }

        order.Status = Enums.OrderStatus.RequiresUserAction;
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation("Order with id: {OrderId} changed status to requires user action", order.Id);
    }

    public async Task SetOrderToProcessing(string paymentIntentId)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var order = await _dbContext.Orders
            .FirstOrThrowAsync(o => o.StripePaymentIntentId == paymentIntentId);

        if (order.Status is not (Enums.OrderStatus.AwaitingPaymentInfo or Enums.OrderStatus.RequiresUserAction or Enums.OrderStatus.Processing))
        {
            _logger.LogCritical("Order is in status {OrderStatus}, an illegal status for setting the order to processing", order.Status);
            throw new BadOrderStatusException(
                $"Order is in status {order.Status}, an illegal status for setting the order to processing");
        }

        order.Status = Enums.OrderStatus.Processing;
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation("Order with id: {OrderId} changed status to processing", order.Id);
    }

    public async Task SetOrderToAwaitingPaymentInfo(string paymentIntentId)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var order = await _dbContext.Orders
            .FirstOrThrowAsync(o => o.StripePaymentIntentId == paymentIntentId);

        if (order.Status is not (Enums.OrderStatus.AwaitingPaymentInfo or Enums.OrderStatus.RequiresUserAction or Enums.OrderStatus.Processing))
        {
            _logger.LogCritical("Order is in status {OrderStatus}, an illegal status for setting the order to awaiting payment info", order.Status);
            throw new BadOrderStatusException(
                $"Order is in status {order.Status}, an illegal status for setting the order to awaiting payment info");
        }

        order.Status = Enums.OrderStatus.AwaitingPaymentInfo;
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation("Order with id: {OrderId} changed status to awaiting payment info", order.Id);
    }

    public async Task FulfillOrder(string paymentIntentId)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        var order = await _dbContext.Orders
            .Include(o => o.Purchaser)
            .Include(o => o.TicketType)
            .Include(o => o.AffiliateCode)
            .Include(o => o.MarketplaceListing!.Ticket)
            .FirstOrThrowAsync(o => o.StripePaymentIntentId == paymentIntentId);

        if (!IsValidStatusForFulfillingOrder(order.Status))
        {
            _logger.LogCritical("Order is in status {OrderStatus}, an illegal status for fulfilling the order", order.Status);
            throw new BadOrderStatusException(
                $"Order is in status {order.Status}, an illegal status for fulfilling the order");
        }

        UpdateStatusAndFulfillOrder(order);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        await SendOrderEmails(order, order.Purchaser);

        _logger.LogInformation("Order with id: {OrderId} changed status to completed", order.Id);
    }

    private async Task SendOrderEmails(Order order, ApplicationUser? purchaser)
    {
        if (purchaser != null)
        {
            NotificationService.SendTicketPurchaseCompleteEmailWithHangfire(purchaser, order);

            var customEventTemplateId = await _dbContext.Orders
                .Where(o => o.Id == order.Id)
                .Select(o => o.TicketType!.Event!.CustomEmailTemplateId)
                .FirstOrDefaultAsync();

            if (customEventTemplateId != null)
                NotificationService.SendCustomEventEmailWithHangfire(purchaser, customEventTemplateId);
        }
    }

    private async Task SendOrderEmails(Order order, string purchaserUserId)
    {
        var purchaser = await _dbContext.Users.FirstOrThrowAsync(u => u.Id == purchaserUserId);
        await SendOrderEmails(order, purchaser);
    }

    private async Task SendOrderEmails(Order order, string purchaserUserId, Event dbEvent)
    {
        var purchaser = await _dbContext.Users.FirstOrThrowAsync(u => u.Id == purchaserUserId);

        NotificationService.SendTicketPurchaseCompleteEmailWithHangfire(purchaser, order);

        if (dbEvent.CustomEmailTemplateId != null)
            NotificationService.SendCustomEventEmailWithHangfire(purchaser, dbEvent.CustomEmailTemplateId);
    }

    private bool IsValidStatusForFulfillingOrder(Enums.OrderStatus orderStatus)
    {
        return orderStatus is Enums.OrderStatus.AwaitingPaymentInfo or Enums.OrderStatus.RequiresUserAction or Enums.OrderStatus.Processing;
    }

    public async Task CancelExpiredOrders()
    {
        var counter = 0;

        var currentDatetime = DateTimeOffset.UtcNow;
        var currentTimeMinusTimeoutLimit = currentDatetime.AddMinutes(-_orderTimeoutMinutes);

        // Putting all of this in a RepeatableRead transaction is potentially problematic for performance in scenarios
        // with hundreds or thousands of transactions that are to be cancelled.
        // It can lead to locking for a long time
        // Alternatives are (with other cons) are:
        // Queuing Hangfire tasks individually (directly from the PlaceOrder method with a timer or from a worker without a timer)
        // Fetching e.g. 10 orders at a time (same as in the pagination) and saving the changes between each pass
        // Fetching all IDs in one transaction and then just looping over them, fetching the ticket type etc. after that
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        var orders = _dbContext.Orders
            .Where(o => o.Status == Enums.OrderStatus.AwaitingPaymentInfo)
            .Where(o => currentTimeMinusTimeoutLimit > o.CreatedAt)
            .Include(o => o.TicketType)
            .Include(o => o.MarketplaceListing)
            .AsAsyncEnumerable();

        await foreach (var order in orders)
        {
            await UpdateStatusAndCancelOrder(order);
            counter++;
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        if (counter > 0)
        {
            _logger.LogInformation("Finished processing cancelling of expired orders. Total number {Number}", counter);
        }
    }

    /// <summary>
    /// Does not save changes to the db
    /// MarketplaceListing & TicketType should be included on the order
    /// </summary>
    private void UpdateStatusAndFulfillOrder(Order order)
    {
        if (order.SecondaryMarketOrder)
        {
            order.MarketplaceListing!.ReservedAt = null;
            order.MarketplaceListing.FulfilledByOrder = order;
            order.MarketplaceListing.SoldAt = DateTimeOffset.UtcNow;
            order.MarketplaceListing.BuyerId = order.PurchaserId;
            order.MarketplaceListing.Ticket!.ActiveMarketplaceListingId = null;
            ReassignTicketForOrder(order);
        }
        else
        {
            order.TicketType!.ReservedTickets -= order.TicketQuantity;
            order.TicketType.TicketsSold += order.TicketQuantity;
            CreateTicketsForOrder(order);
        }

        order.CompletedAt = DateTimeOffset.UtcNow;
        order.Status = Enums.OrderStatus.Completed;
    }

    /// <summary>
    /// Does not save the changes to the database
    /// </summary>
    private void CreateTicketsForOrder(Order order)
    {
        order.Tickets ??= new List<Ticket>(order.TicketQuantity);
        for (var i = 0; i < order.TicketQuantity; i++)
        {
            var ticket = new Ticket
            {
                OwnerId = order.PurchaserId,
                TicketTypeId = order.TicketTypeId,
                OriginalOrder = order,
                LatestOrder = order,
            };
            order.Tickets.Add(ticket);
        }

        _logger.LogInformation("Created {TicketQuantity} tickets for order with id: {OrderId}",
            order.TicketQuantity, order.Id);
    }

    /// <summary>
    /// Does not save the changes to the database
    /// </summary>
    private void ReassignTicketForOrder(Order order)
    {
        if (order.MarketplaceListing?.Ticket == null)
        {
            const string errorText = "Ticket missing when assigning ticket from marketplace listing!";
            _logger.LogCritical(errorText);
            throw new Exception(errorText);
        }

        order.MarketplaceListing.Ticket.OwnerId = order.PurchaserId;
        order.MarketplaceListing.Ticket.LatestOrder = order;
        order.Tickets = new List<Ticket> { order.MarketplaceListing.Ticket };

        _logger.LogInformation("Assigned ticket with id {TicketId} to user {UserId}",
            order.MarketplaceListing.TicketId, order.PurchaserId);
    }

    /// <summary>
    /// Does not save changes to the db
    /// Note: Failed stripe payment intents do NOT lead to orders themselves being failed
    /// (as payment intents can be re-used)
    ///
    /// TicketType and MarketPlaceListing should be included to use this method
    /// </summary>
    private void UpdateStatusAndFailOrder(Order order)
    {
        if (order.SecondaryMarketOrder)
        {
            order.MarketplaceListing!.ReservedAt = null;
        }
        else
        {
            order.TicketType!.ReservedTickets -= order.TicketQuantity;
        }

        order.Status = Enums.OrderStatus.Failed;
    }

    /// <summary>
    /// Does not save changes to the db
    /// </summary>
    private async Task UpdateStatusAndCancelOrder(Order order)
    {
        // The db order should never be cancelled without the payment intent being successfully cancelled first
        if (order.StripePaymentIntentId != null)
            await _paymentService.CancelPaymentIntent(order.StripePaymentIntentId);

        if (order.SecondaryMarketOrder)
        {
            order.MarketplaceListing!.ReservedAt = null;
        }
        else
        {
            order.TicketType!.ReservedTickets -= order.TicketQuantity;
        }
        order.Status = Enums.OrderStatus.Cancelled;
    }
}
