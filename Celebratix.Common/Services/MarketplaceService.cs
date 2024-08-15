using System.Data;
using Celebratix.Common.Configs;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.User.Marketplace;
using Celebratix.Common.Models.DTOs.User.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Celebratix.Common.Services;

public class MarketplaceService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly MarketplaceConfig _marketplaceConfig;
    private readonly ILogger<MarketplaceService> _logger;

    public MarketplaceService(CelebratixDbContext dbContext, ILogger<MarketplaceService> logger, IOptions<MarketplaceConfig> marketplaceConfig)
    {
        _dbContext = dbContext;
        _logger = logger;
        _marketplaceConfig = marketplaceConfig.Value;
    }

    public async Task<PagedResultDto<MarketplaceListingBasicDto>> GetListingsForEventAsUserDtos(int eventId, Guid? ticketTypeId, string? currentUserId, int page, int pageSize)
    {
        var baseQuery = _dbContext.MarketplaceListings
            .Include(ml => ml.Currency)
            .Include(ml => ml.Seller)
            .Include(ml => ml.Ticket!.TicketType!.Event!.Image)
            .Include(ml => ml.Ticket!.TicketType!.Event!.Category)
            .Where(ml => ml.Ticket!.TicketType!.EventId == eventId)
            .Where(ml => ml.SoldAt == null && ml.ReservedAt == null && !ml.Cancelled);

        if (ticketTypeId != null)
        {
            baseQuery = baseQuery.Where(ml => ml.Ticket!.TicketTypeId == ticketTypeId);
        }

        var listings = await baseQuery
            .OrderBy(ml => ml.ListingPrice)
            .Select(ml => new MarketplaceListingBasicDto(ml, currentUserId))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);

        return listings;
    }

    public async Task<PagedResultDto<MarketplaceListingBasicDto>> GetListingsForUserAsDto(string currentUserId, int page, int pageSize, bool? sold)
    {
        var baseQuery = _dbContext.MarketplaceListings
            .Include(ml => ml.Currency)
            .Include(ml => ml.Ticket!.TicketType!.Event!.Image)
            .Include(ml => ml.Ticket!.TicketType!.Event!.Category)
            .Where(ml => ml.SellerId == currentUserId);

        if (sold == true)
        {
            baseQuery = baseQuery.Where(ml => ml.SoldAt != null);
        }
        else if (sold == false)
        {
            baseQuery = baseQuery.Where(ml => ml.SoldAt == null);
        }

        var listings = await baseQuery
            .OrderByDescending(ml => ml.CreatedAt)
            .Select(ml => new MarketplaceListingBasicDto(ml, currentUserId))
            .AsSplitQuery()
            .ToPagedResult(page, pageSize);

        return listings;
    }

    public async Task<MarketplaceListingDetailedDto> GetListingAsDetailedUserDto(Guid listingId, string? currentUserId = null)
    {
        var listing = await _dbContext.MarketplaceListings
            .Include(ml => ml.Currency)
            .Include(ml => ml.Seller)
            .Include(ml => ml.Ticket!.TicketType!.Event!.Image)
            .Include(ml => ml.Ticket!.TicketType!.Event!.Category)
            .FirstOrThrowAsync(ml => ml.Id == listingId);
        return new MarketplaceListingDetailedDto(listing, currentUserId);
    }

    public async Task<MarketplaceListingDetailedDto> CreateListing(MarketplaceListingCreateDto dto, string userId)
    {
        // You can only create listings if payouts is enabled on your account
        var payoutsEnabled = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.StripePayoutRequirementsFulfilled)
            .FirstOrDefaultAsync();

        if (!payoutsEnabled)
        {
            throw new PayoutsNotEnabledOnUserException();
        }

        var ticketType = await _dbContext.Tickets
            .Include(t => t.TicketType!.Event!.Currency)
            .Where(t => t.Id == dto.TicketId)
            .Select(t => t.TicketType!)
            .FirstOrThrowAsync();
        var currency = ticketType.Event!.Currency!;
        dto.ListingPrice = Math.Round(dto.ListingPrice, currency.DecimalPlaces);

        // Check so the end time of the event hasn't already passed
        if (DateTimeOffset.UtcNow > ticketType.Event!.EndDate)
        {
            throw new EventEndTimePassedException();
        }

        var originalPrice = await _dbContext.Tickets
            .Where(t => t.Id == dto.TicketId)
            .Select(t => t.OriginalOrder!.Amount)
            .FirstOrDefaultAsync();

        var maxSellingPrice = originalPrice * _marketplaceConfig.MaxPriceOverOriginal;
        var serviceFee = maxSellingPrice * _marketplaceConfig.ServiceFeeFraction;
        var transactionFee = (maxSellingPrice + serviceFee) * _marketplaceConfig.SecondaryFeeFraction;

        var min = Math.Round(currency.MinMarketplaceListingPrice, currency.DecimalPlaces);
        var max = Math.Round(maxSellingPrice + serviceFee + transactionFee, currency.DecimalPlaces);
        if (dto.ListingPrice < min || dto.ListingPrice > max)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.ListingPrice), $"The price {dto.ListingPrice} isn't in the allowed range {min}-{max}"); //
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        var ticket = await _dbContext.Tickets
            .Where(t => t.OwnerId == userId)
            .FirstOrThrowAsync(t => t.Id == dto.TicketId);

        var action = new TicketQrActions(ticket, userId, "");
        switch (action.Marketplace.UnavailableReason)
        {
            case null:
                break;
            case MarketplaceUnavailableReason.CheckedIn:
                throw new TicketAlreadyScannedException();
            case MarketplaceUnavailableReason.TicketTransferOffer:
                throw new TicketInTransferOfferException();
            case MarketplaceUnavailableReason.SoldOrTransferred:
                throw new TicketSoldOrTransferredException();
            case MarketplaceUnavailableReason.EventPassed:
                throw new EventEndTimePassedException();
            default:
                throw new Exception($"Unknown {nameof(MarketplaceUnavailableReason)}: {action.Marketplace.UnavailableReason}");
        };
        if (ticket.ActiveMarketplaceListingId != null)
            throw new TicketInMarketplaceListingException();

        var listing = new MarketplaceListing
        {
            Ticket = ticket,
            ListingPrice = dto.ListingPrice,
            ServiceFee = Math.Round(dto.ListingPrice * _marketplaceConfig.ServiceFeeFraction, currency.DecimalPlaces, MidpointRounding.ToZero),
            SellerId = userId,
            CurrencyId = ticketType.Event.CurrencyId
        };

        ticket.ActiveMarketplaceListing = listing;

        _dbContext.MarketplaceListings.Add(listing);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation("Created a new Marketplace Listing for user: {UserId}, ListingId: {ListingId}",
            userId, listing.Id);

        return await GetListingAsDetailedUserDto(listing.Id, userId);
    }

    public async Task<MarketplaceListingDetailedDto> CancelListing(string currentUserId, Guid listingId)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        var listing = await _dbContext.MarketplaceListings
            .Include(ml => ml.Ticket)
            .Where(ml => ml.SellerId == currentUserId)
            .FirstOrThrowAsync(ml => ml.Id == listingId);

        if (listing.Available)
        {
            listing.Ticket!.ActiveMarketplaceListingId = null;
            listing.Cancelled = true;
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Cancelled Marketplace Listing for user: {UserId}, ListingId: {ListingId}",
                currentUserId, listing.Id);
        }
        else
        {
            throw new ListingNotAvailableException("Listing is not available to cancel");
        }

        return await GetListingAsDetailedUserDto(listingId, currentUserId);
    }

    public async Task CancelListingsForPastEvents()
    {
        var counter = 0;
        var currentDatetime = DateTimeOffset.UtcNow;

        // Putting all of this in a RepeatableRead transaction is potentially problematic for performance in scenarios
        // with hundreds or thousands of listings that are to be cancelled.
        // It can lead to locking for a long time
        // Maybe add "max 1000" and run it every minute or something?
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

        var listings = _dbContext.MarketplaceListings
            .Include(ml => ml.Ticket)
            .Where(ml => ml.Cancelled == false)
            .Where(ml => ml.SoldAt == null)
            .Where(ml => ml.ReservedAt == null)
            .Where(ml => currentDatetime > ml.Ticket!.TicketType!.Event!.EndDate)
            .AsAsyncEnumerable();

        await foreach (var listing in listings)
        {
            listing.Cancelled = true;
            listing.Ticket!.ActiveMarketplaceListingId = null;
            counter++;
        }

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        if (counter > 0)
            _logger.LogInformation("Finished processing cancelling of listing of past events. Total number {Number}", counter);
    }
}
