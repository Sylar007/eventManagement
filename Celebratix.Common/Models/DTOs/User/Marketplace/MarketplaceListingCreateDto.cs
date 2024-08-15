using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.User.Marketplace;

public class MarketplaceListingCreateDto
{
    public Guid TicketId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    public decimal ListingPrice { get; set; }

    // We do not currently allow user to sell in anything but the events currency
    //public string CurrencyCode { get; set; }
}
