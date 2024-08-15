using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs;

public class CategoryDto
{
    public CategoryDto(Category dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
    }

    public int Id { get; set; }

    public string Name { get; set; }
}
