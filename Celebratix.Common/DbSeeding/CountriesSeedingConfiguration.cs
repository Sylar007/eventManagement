using System.Globalization;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PhoneNumbers;

namespace Celebratix.Common.DbSeeding;

public class CountriesSeedingConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        var countries = new List<Country>();

        // Azure/Dotnet is buggy when it comes to cultures. Especially on linux...
        // https://stackoverflow.com/questions/41851613/culture-is-suddenly-not-supported-anymore-on-azure-web-app
        // This is the reason for the workaround below

        //var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));

        var regions = new List<RegionInfo>();

        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            try
            {
                regions.Add(new RegionInfo(culture.LCID));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        foreach (var region in regions)
        {
            if (countries.All(p => p.Name != region.EnglishName)
                && !NonCountries.Contains(region.TwoLetterISORegionName))
            {
                var country = region.ToCountry();

                country.CallingCode = FindCountryCode(country.Id);

                country.Enabled = EnabledCountries.Contains(country.Id);
                countries.Add(country);
            }
        }

        builder.HasData(countries);
    }

    private static readonly List<string> NonCountries = new()
    {
        "001", // World
        "029", // Caribbean
        "419", // Latin america
    };

    private static readonly List<string> EnabledCountries = new()
    {
        "SE",
        "NL"
    };

    private static string FindCountryCode(string countryShortCode)
    {

        var phoneUtil = PhoneNumberUtil.GetInstance();
        return "+" + phoneUtil.GetCountryCodeForRegion(countryShortCode.ToUpper());

    }
}
