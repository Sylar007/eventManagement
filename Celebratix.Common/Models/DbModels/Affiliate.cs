using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Celebratix.Common.Models.DbModels;

[Index(nameof(Code), IsUnique = true)]
public class Affiliate : DbModelBase
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    [ForeignKey(nameof(Channel))]
    public Guid ChannelId { get; set; }
    public Channel? Channel { get; set; } = null!;

    public string? CreatorId { get; set; }
    public ApplicationUser? Creator { get; set; }

    public string Code { get; set; } = null!;

    public ICollection<Order>? Orders { get; set; }

    /// <summary>
    /// Requires Orders to be included
    /// </summary>
    [NotMapped]
    public IEnumerable<Order>? FulfilledOrders => Orders?.Where(t => t.Status == Enums.OrderStatus.Completed);
    // TODO: maybe do in same way that EventTicketTypes were done 

    public int Views { get; set; } = 0;

    /// <summary>
    /// Tickets bought with the code
    /// Requires Orders to be included
    /// </summary>
    [NotMapped]
    public int? TicketsBought => FulfilledOrders?.Select(o => o.TicketQuantity).Sum();

    /// <summary>
    /// Sum of base cost of all orders bought with this affiliate code
    /// Requires Orders to be included
    /// </summary>
    [NotMapped]
    public decimal BaseRevenue => FulfilledOrders?.Sum(t => t.BaseAmount) ?? decimal.Zero;

    /// <summary>
    /// Sum of service fee of all orders bought with this affiliate code
    /// Requires Orders to be included
    /// </summary>
    [NotMapped]
    public decimal ServiceRevenue => FulfilledOrders?.Sum(t => t.ServiceAmount) ?? decimal.Zero;

    /// <summary>
    /// Sum of application fee of all orders bought with this affiliate code
    /// Requires Orders to be included
    /// </summary>
    [NotMapped]
    public decimal ApplicationRevenue => FulfilledOrders?.Sum(t => t.ApplicationAmount) ?? decimal.Zero;
}
