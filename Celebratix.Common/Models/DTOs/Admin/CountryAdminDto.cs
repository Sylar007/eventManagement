using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Admin;

// The "normal" country dto could've been extended, but it's probably good to keep complexity to the very minimum
public class CountryAdminDto
{
    public CountryAdminDto(Country country)
    {
        Id = country.Id;
        ISO3 = country.ISO3;
        Name = country.Name;
        Enabled = country.Enabled;
        CallingCode = country.CallingCode;
    }

    /// <summary>
    /// The ISO 3166 two-letter code for the country/region.
    /// </summary>
    public string Id { get; set; } // I.e. ISO2

    /// <summary>
    /// The ISO 3166 three-letter code for the country/region.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string ISO3 { get; set; }

    /// <summary>
    /// The full name of the country/region in English,
    /// </summary>
    public string Name { get; set; }

    public string CallingCode { get; set; }

    /// <summary>
    /// If the country is supported
    /// </summary>
    public bool Enabled { get; set; }
}
