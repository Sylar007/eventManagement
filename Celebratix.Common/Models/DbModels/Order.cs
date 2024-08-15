using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

/// <summary>
/// Can represent many things
/// Either a purchase directly from Celebratix, i.e. the Primary Market
/// Or a marketplace listing purchase
/// Or a accepted ticket transfer
/// </summary>
public class Order : DbModelBase
{
    public Guid Id { get; set; }

    public Enums.OrderStatus Status { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Nullable to support deletion of users (without deleting the order)
    /// </summary>
    [ForeignKey(nameof(ApplicationUser))]
    public string? PurchaserId { get; set; }
    public ApplicationUser? Purchaser { get; set; }

    [ForeignKey(nameof(TicketType))]
    public Guid TicketTypeId { get; set; }
    public EventTicketType? TicketType { get; set; }

    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
    public Event? Event { get; set; }

    [ForeignKey(nameof(AffiliateCode))]
    public Guid? AffiliateCodeId { get; set; }
    public Affiliate? AffiliateCode { get; set; }

    [ForeignKey(nameof(MarketplaceListing))]
    public Guid? MarketplaceListingId { get; set; }
    public MarketplaceListing? MarketplaceListing { get; set; }

    public bool SecondaryMarketOrder => MarketplaceListingId != null;

    [ForeignKey(nameof(TicketTransferOffer))]
    public Guid? TicketTransferOfferId { get; set; }
    public TicketTransferOffer? TicketTransferOffer { get; set; }

    public bool TicketTransferOfferOrder => TicketTransferOfferId != null;

    public int TicketQuantity { get; set; }

    /// <summary>
    /// Including VAT
    /// </summary>
    [NotMapped]
    public decimal Amount => BaseAmount + ServiceAmount + ApplicationAmount;

    [Column(TypeName = "decimal(18,6)")]
    public decimal BaseAmount { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal ServiceAmount { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal ApplicationAmount { get; set; }

    /// <summary>
    /// As a fraction. E.g. 0.12
    /// </summary>
    [Column(TypeName = "decimal(4,4)")]
    public decimal Vat { get; set; }

    public decimal VatAmount => Amount * Vat;

    [ForeignKey(nameof(Currency))]
    public string? CurrencyId { get; set; }
    public Currency? Currency { get; set; }

    /// <summary>
    /// The payment intent ID
    /// </summary>
    public string? StripePaymentIntentId { get; set; }

    /// <summary>
    /// The tickets are only actually created once the purchase has been completed
    /// Contains Tickets that have e.g. been transferred  or sold from the order too
    /// </summary>
    [InverseProperty(nameof(Ticket.Orders))]
    public ICollection<Ticket>? Tickets { get; set; }

    /// <summary>
    /// The inverse prop of the "OriginalOrder" property
    /// Will probably never be used directly
    /// </summary>
    [InverseProperty(nameof(Ticket.OriginalOrder))]
    public ICollection<Ticket>? OriginalTickets { get; set; }

    /// <summary>
    /// The inverse prop of the "LatestOrder" property
    /// Will probably never be used directly
    /// </summary>
    [InverseProperty(nameof(Ticket.LatestOrder))]
    public ICollection<Ticket>? InverseTicketLatestOrder { get; set; }

    [NotMapped]
    public Enums.OrderType Type
    {
        get
        {
            if (TicketTransferOfferOrder)
                return Enums.OrderType.Transfer;
            if (SecondaryMarketOrder)
                return Enums.OrderType.Marketplace;
            return Enums.OrderType.PrimaryMarket;
        }
    }
}
