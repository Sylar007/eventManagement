using System.Globalization;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Extensions;

public static class ModelExtensions
{
    public static Country ToCountry(this RegionInfo regionInfo)
    {
        var country = new Country
        {
            Id = regionInfo.TwoLetterISORegionName,
            ISO3 = regionInfo.ThreeLetterISORegionName,
            Name = regionInfo.EnglishName
        };

        return country;
    }
}
