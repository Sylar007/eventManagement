using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Celebratix.Common.Models.DbModels;

/// <summary>
/// Does not hold the actual file itself (as that is not to be saved in the database)
/// </summary>
public class FileDbModel : DbModelBase
{
    /// <summary>
    /// Used both as a PK and as the storage identifier (i.e. the file name on e.g azure)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid StorageIdentifier { get; set; }

    public string OriginalFileName { get; set; } = null!;
}
