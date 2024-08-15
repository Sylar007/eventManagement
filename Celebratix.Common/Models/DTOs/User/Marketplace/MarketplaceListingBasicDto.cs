using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;

namespace Celebratix.Common.Models.DTOs.User.Marketplace;

public class MarketplaceListingBasicDto
{
    /// <summary>
    /// Required includes:
    /// Currency
    /// Ticket.TicketType.Event.Category + Image
    /// </summary>
    public MarketplaceListingBasicDto(MarketplaceListing dbModel, string? currentUserId = null)
    {
        Id = dbModel.Id;
        ListingPrice = dbModel.ListingPrice;
        Currency = new CurrencyDto(dbModel.Currency!);
        Event = new EventBasicDto(dbModel.Ticket!.TicketType!.Event!);
        TicketTypeId = dbModel.Ticket.TicketTypeId;
        TicketTypeName = dbModel.Ticket.TicketType.Name;
        Cancelled = dbModel.Cancelled;
        Reserved = dbModel.Reserved;
        Sold = dbModel.Sold;
        Available = dbModel.Available;
        Seller = dbModel.Seller == null ? null : new PublicProfileDto(dbModel.Seller);
        if (currentUserId != null)
        {
            OwnListing = dbModel.SellerId == currentUserId;
        }
    }

    public Guid Id { get; set; }
    public decimal ListingPrice { get; set; }
    public CurrencyDto Currency { get; set; }

    public EventBasicDto Event { get; set; }

    public PublicProfileDto? Seller { get; set; }

    public Guid TicketTypeId { get; set; }

    public string TicketTypeName { get; set; }

    public bool Cancelled { get; set; }
    public bool Reserved { get; set; }
    public bool Sold { get; set; }
    public bool Available { get; set; }

    public bool? OwnListing { get; set; }
}
