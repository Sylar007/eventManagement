using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

public class Currency : DbModelBase
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [MaxLength(5)]
    public string Code { get; set; } = null!;

    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(5)]
    public string Symbol { get; set; } = null!;

    public int DecimalPlaces { get; set; }

    /// <summary>
    /// To guarantee payouts for all listings, this should be min payout for the currency by stripe + the service fee
    /// </summary>
    [Column(TypeName = "decimal(18,6)")]
    public decimal MinMarketplaceListingPrice { get; set; }

    public bool Enabled { get; set; } = false;
}
