using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.User.Orders;

public class PrimaryMarketOrderCreateDto
{
    public Guid TicketTypeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity have to be bigger than 0")]
    public int TicketQuantity { get; set; }

    /// <summary>
    /// The actual code, not the ID
    /// </summary>
    public string? AffiliateCode { get; set; }
}
