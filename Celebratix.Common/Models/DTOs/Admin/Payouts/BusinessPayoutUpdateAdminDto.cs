using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.Admin.Payouts;

public class BusinessPayoutUpdateAdminDto
{
    [Range(0, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    public decimal Amount { get; set; }

    public string? Comment { get; set; }
}
