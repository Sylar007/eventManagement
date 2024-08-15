using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User.Marketplace;

public class MarketplaceListingDetailedDto : MarketplaceListingBasicDto
{
    /// <summary>
    /// Required includes:
    /// Currency
    /// Ticket.TicketType.Event.Category + Image
    /// </summary>
    public MarketplaceListingDetailedDto(MarketplaceListing dbModel, string? currentUserId = null) : base(dbModel, currentUserId)
    {
        SoldAt = dbModel.SoldAt;
        ServiceFee = dbModel.ServiceFee;
    }

    public decimal ServiceFee { get; set; }

    public DateTimeOffset? SoldAt { get; set; }
}
