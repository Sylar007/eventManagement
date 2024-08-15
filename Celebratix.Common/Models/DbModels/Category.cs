using Microsoft.EntityFrameworkCore;

namespace Celebratix.Common.Models.DbModels;

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Event>? Events { get; set; }
}
