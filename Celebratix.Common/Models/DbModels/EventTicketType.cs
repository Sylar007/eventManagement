using Celebratix.Common.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

[Index(nameof(LinkCode))]
public class EventTicketType : DbModelBase
{
    public Guid Id { get; set; }

    /// <summary>
    /// Tickets in the frontend are sorted by this index (ascending)
    /// </summary>
    public int SortIndex { get; set; } = 0;

    public string Name { get; set; } = null!;

    /// <summary>
    /// Set by the event organizer. The base price of the ticket, without any fees
    /// </summary>
    [Column(TypeName = "decimal(18,6)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Set by the event organizer. An extra fee that the organizer can set for "service"
    /// </summary>
    [Column(TypeName = "decimal(18,6)")]
    public decimal ServiceFee { get; set; } = 0.3m;

    /// <summary>
    /// Celebratix can overwrite the application fee for a specific event instead of the application fee set at the buisness level
    /// </summary>
    [Column(TypeName = "decimal(18,6)")]
    public decimal? ApplicationFeeOverwrite { get; set; }

    /// <summary>
    /// Set by Celebratix. The fee that Celebratix takes for each ticket sold.
    /// For now we set this at the event organizer level and it's propagated to all ticket types
    /// So
    /// </summary>
    [NotMapped]
    public decimal ApplicationFee => IsFree ? decimal.Zero : ApplicationFeeOverwrite ?? Event?.Business?.ApplicationFee ?? 0.7m; // TODO: use global?

    /// <summary>
    /// If this ticket is free, if true, the user is not redirected to the payment page
    /// </summary>
    [NotMapped]
    public bool IsFree => Price + ServiceFee == decimal.Zero;

    /// <summary>
    /// The price that the customer pays
    /// If the ticket is free this is 0
    /// If the ticket is not free this is Price + ServiceFee + ApplicationFee
    /// </summary>
    [NotMapped]
    public decimal TotalPrice => IsFree ? decimal.Zero : Price + ServiceFee + ApplicationFee;

    /// <summary>
    /// Only used for generating a report at the end of the event
    /// </summary>
    [Column(TypeName = "decimal(4,4)")]
    public decimal CustomVat = 0.09m;

    [NotMapped]
    public AmountDto? Revenue { get; set; }

    public int? MaxTicketsAvailable { get; set; }

    public int ReservedTickets { get; set; } = 0;

    public int TicketsSold { get; set; } = 0;

    /// <summary>
    /// This should always be the sum of Tickets for the TicketType where checked in = true
    /// For performance reasons a counter like this is used though
    /// </summary>
    public int TicketsCheckedIn { get; set; }

    [NotMapped]
    public int? AvailableTickets => MaxTicketsAvailable != null ? Math.Max(0, MaxTicketsAvailable.Value - ReservedTickets - TicketsSold) : null;

    public int MinTicketsPerPurchase { get; set; } = 1;

    public int MaxTicketsPerPurchase { get; set; } = 10;

    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
    public Event? Event { get; set; }

    public bool PubliclyAvailable { get; set; } = true;

    public DateTimeOffset? AvailableFrom { get; set; }

    public DateTimeOffset? AvailableUntil { get; set; }

    public string? LinkCode;

    public bool OnlyAffiliates { get; set; } = false;

    [ForeignKey(nameof(Image))]
    public Guid? TicketImageId { get; set; }
    public ImageDbModel? Image { get; set; }

    public int? CategoryId { get; set; }

    public bool HideSoldOut { get; set; } = false;
}
