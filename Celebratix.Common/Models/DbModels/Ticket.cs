using System.ComponentModel.DataAnnotations.Schema;
using Celebratix.Common.Models.DTOs.User.Tickets;

namespace Celebratix.Common.Models.DbModels;

public class Ticket : DbModelBase
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(TicketType))]
    public Guid TicketTypeId { get; set; }
    public EventTicketType? TicketType { get; set; }

    /// <summary>
    /// Nullable to support deletion of users (without deleting the ticket)
    /// </summary>
    [ForeignKey(nameof(Owner))]
    public string? OwnerId { get; set; } = null!;

    public ApplicationUser? Owner { get; set; }

    /// <summary>
    /// This relationship is not necessarily the same as MarketplaceListing.TicketId
    /// A ticket can have many MarketplaceListings in it's history, but only ever one active one
    /// </summary>
    [ForeignKey(nameof(ActiveMarketplaceListing))]
    public Guid? ActiveMarketplaceListingId { get; set; }
    public MarketplaceListing? ActiveMarketplaceListing { get; set; }

    /// <summary>
    /// This relationship is not necessarily the same as TicketTransferOffer.TicketId
    /// A ticket can have many TicketTransferOffers in it's history, but only ever one active one
    /// </summary>
    [ForeignKey(nameof(ActiveTicketTransferOffer))]
    public Guid? ActiveTicketTransferOfferId { get; set; }
    public TicketTransferOffer? ActiveTicketTransferOffer { get; set; }

    /// <summary>
    /// Set to true when ticket is scanned
    /// </summary>
    public bool CheckedIn { get; set; }
    public Enums.RefundType Refund { get; set; }

    [NotMapped, Obsolete($"Use {nameof(ActiveMarketplaceListingId)} instead")]
    public bool IsListed => ActiveMarketplaceListingId != null;

    [NotMapped, Obsolete($"Use {nameof(ActiveTicketTransferOfferId)} instead")]
    public bool IsOffered => ActiveTicketTransferOfferId != null;

    /// <summary>
    /// The original order of the ticket, i.e. the one where the tickets were created in
    /// This is nullable to potentially support manually assigning tickets to users
    /// </summary>
    [ForeignKey(nameof(OriginalOrder))]
    public Guid? OriginalOrderId { get; set; }
    public Order? OriginalOrder { get; set; }

    /// <summary>
    /// The latest order of the ticket, to track whether a ticket was transferred
    /// </summary>
    [ForeignKey(nameof(LatestOrder))]
    public Guid LatestOrderId { get; set; }
    public Order? LatestOrder { get; set; }

    /// <summary>
    /// All orders the ticket is a part (current + all previous)
    /// </summary>
    [InverseProperty(nameof(Order.Tickets))]
    public ICollection<Order>? Orders { get; set; }

    // QR data. Can't be based on GUID as that can't prove ownership (browser history etc.). Timestamped qr etc.?
}
