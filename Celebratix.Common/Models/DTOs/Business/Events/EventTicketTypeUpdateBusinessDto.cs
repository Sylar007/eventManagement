using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class EventTicketTypeUpdateBusinessDto
{
    public string Name { get; set; } = null!;

    [Range(0, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    [Precision(18, 6)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    [Precision(18, 6)]
    public decimal ServiceFee { get; set; }

    /// <summary>
    /// As a fraction. E.g. 0.12
    /// </summary>
    [Range(0, 1, ErrorMessage = "Only values between 0 and 1 are valid")]
    [Precision(4, 4)]
    public decimal? CustomVat { get; set; }

    public bool PubliclyAvailable { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    public int? MaxTicketsAvailable { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    public int MinTicketsPerPurchase { get; set; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    public int MaxTicketsPerPurchase { get; set; } = 10;

    public DateTimeOffset? AvailableFrom { get; set; }

    public DateTimeOffset? AvailableUntil { get; set; }

    public string? LinkCode { get; set; }

    public bool OnlyAffiliates { get; set; } = false;
}
