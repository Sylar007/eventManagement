using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

public class MarketplaceListing : DbModelBase
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Ticket))]
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    /// <summary>
    /// Including service fee
    /// </summary>
    [Column(TypeName = "decimal(18,6)")]
    public decimal ListingPrice { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal ServiceFee { get; set; }

    [ForeignKey(nameof(Currency))]
    public string CurrencyId { get; set; } = null!;
    public Currency? Currency { get; set; }

    /// <summary>
    /// An Order can have many MarketplaceListings (i.e. if they fail)
    /// This represent the final order that actually fulfilled the listing
    /// </summary>
    [ForeignKey(nameof(FulfilledByOrder))]
    public Guid? FulfilledByOrderId { get; set; }
    public Order? FulfilledByOrder { get; set; }

    /// <summary>
    /// Nullable as user can potentially be deleted
    /// </summary>
    [ForeignKey(nameof(Seller))]
    public string? SellerId { get; set; }
    public ApplicationUser? Seller { get; set; }

    [ForeignKey(nameof(Buyer))]
    public string? BuyerId { get; set; }
    public ApplicationUser? Buyer { get; set; }

    public DateTimeOffset? ReservedAt { get; set; }

    [NotMapped]
    public bool Reserved => ReservedAt != null;

    public DateTimeOffset? SoldAt { get; set; }

    [NotMapped]
    public bool Sold => SoldAt != null;

    [NotMapped]
    public bool Available => !Sold && !Reserved && !Cancelled;

    public bool Cancelled { get; set; } = false;
}
