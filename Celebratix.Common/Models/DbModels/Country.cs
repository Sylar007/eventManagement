namespace Celebratix.Common.Models.DbModels;

public class Country
{
    /// <summary>
    /// The ISO 3166 two-letter code for the country/region.
    /// </summary>
    public string Id { get; set; } = null!; // I.e. ISO2

    /// <summary>
    /// The ISO 3166 three-letter code for the country/region.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string ISO3 { get; set; } = null!;

    /// <summary>
    /// The full name of the country/region in English,
    /// </summary>
    public string Name { get; set; } = null!;

    public string CallingCode { get; set; } = null!;

    /// <summary>
    /// If the country is supported
    /// </summary>
    public bool Enabled { get; set; } = false;
}
