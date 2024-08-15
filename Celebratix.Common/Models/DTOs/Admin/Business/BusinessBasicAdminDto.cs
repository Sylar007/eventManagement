namespace Celebratix.Common.Models.DTOs.Admin.Business;

public class BusinessBasicAdminDto
{
    /// <summary>
    /// Required includes:
    /// Country
    /// </summary>
    public BusinessBasicAdminDto(DbModels.Business dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
        FacebookPixelId = dbModel.FacebookPixelId;
        Country = new CountryAdminDto(dbModel.Country!);
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? FacebookPixelId { get; set; }

    public CountryAdminDto Country { get; set; }
}
