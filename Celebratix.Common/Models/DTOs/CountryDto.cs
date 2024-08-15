using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs;

public class CountryDto
{
    public CountryDto(Country country)
    {
        Id = country.Id;
        Iso3 = country.ISO3;
        Name = country.Name;
        CallingCode = country.CallingCode;
    }

    /// <summary>
    /// The ISO 3166 two-letter code for the country/region.
    /// </summary>
    public string Id { get; set; } // I.e. ISO2

    /// <summary>
    /// The ISO 3166 three-letter code for the country/region.
    /// </summary>
    public string Iso3 { get; set; }

    /// <summary>
    /// The full name of the country/region in English,
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// E.g. +46
    /// </summary>
    public string CallingCode { get; set; }
}
