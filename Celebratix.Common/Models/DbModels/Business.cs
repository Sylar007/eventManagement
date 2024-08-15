using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

public class Business : DbModelBase
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [ForeignKey(nameof(DbModels.Country))]
    public string? CountryId { get; set; }
    public Country? Country { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal ApplicationFee { get; set; } = 0.7m;

    public string? FacebookPixelId { get; set; }

    public string? EventsPageUrl { get; set; }

    public ICollection<Event>? Events { get; set; }

    [NotMapped]
    public ICollection<Event>? UpcomingEvents =>
        Events?.Where(e => e.EndDate >= DateTimeOffset.UtcNow).ToList();

    [NotMapped]
    public ICollection<Event>? PastEvents =>
        Events?.Where(e => DateTimeOffset.UtcNow >= e.EndDate).ToList();

    public ICollection<BusinessPayout>? Payouts { get; set; }
}
