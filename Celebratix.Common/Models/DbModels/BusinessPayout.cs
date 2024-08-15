using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

public class BusinessPayout : DbModelBase
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Business))]
    public Guid BusinessId { get; set; }
    public Business? Business { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal Amount { get; set; }

    public string? Comment { get; set; }

    // "Cut of date" or something similar to indicate what timespan the payouts is for?
}
