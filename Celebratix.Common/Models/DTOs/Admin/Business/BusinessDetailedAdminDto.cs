namespace Celebratix.Common.Models.DTOs.Admin.Business;

public class BusinessDetailedAdminDto
{
    /// <summary>
    /// Required includes:
    /// Country
    /// </summary>
    public BusinessDetailedAdminDto(DbModels.Business dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
        Country = new CountryAdminDto(dbModel.Country!);
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public CountryAdminDto Country { get; set; }
}
