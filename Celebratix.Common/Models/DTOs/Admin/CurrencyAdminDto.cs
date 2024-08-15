using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Admin;

public class CurrencyAdminDto : CurrencyDto
{
    public CurrencyAdminDto(Currency dbModel) : base(dbModel)
    {
        MinMarketplaceListingPrice = dbModel.MinMarketplaceListingPrice;
    }

    public decimal MinMarketplaceListingPrice { get; set; }
}
