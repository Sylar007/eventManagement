using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.Admin;

public class CurrencyUpdateAdminDto
{
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(5)]
    public string Symbol { get; set; } = null!;

    [Range(0, 10, ErrorMessage = "Only values between 0 and 10 are valid")]
    public int DecimalPlaces { get; set; }

    /// <summary>
    /// To guarantee payouts for all listings, this should be min payout for the currency by stripe + the service fee
    /// </summary>
    public decimal MinMarketplaceListingPrice { get; set; }

    public bool Enabled { get; set; } = false;
}
