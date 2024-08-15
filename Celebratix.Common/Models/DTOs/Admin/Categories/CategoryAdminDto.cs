using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Admin.Categories;

public class CategoryAdminDto
{
    public CategoryAdminDto(Category dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
    }

    public int Id { get; set; }

    public string Name { get; set; }
}
