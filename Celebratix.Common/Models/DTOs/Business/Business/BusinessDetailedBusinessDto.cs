namespace Celebratix.Common.Models.DTOs.Admin.Business; // todo

public class BusinessDetailedBusinessDto
{
    /// <summary>
    /// Required includes:
    /// Country
    /// </summary>
    public BusinessDetailedBusinessDto(DbModels.Business dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
        Country = new CountryDto(dbModel.Country!);
        FacebookPixelId = dbModel.FacebookPixelId;
        EventsPageUrl = dbModel.EventsPageUrl;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? FacebookPixelId { get; set; }

    public string? EventsPageUrl { get; set; }

    public CountryDto Country { get; set; }
}
