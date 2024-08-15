using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

public class RefreshToken : DbModelBase
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// The first of the token family, used to invalidate an entire family at once
    /// </summary>
    public Guid Originator { get; set; }

    public DateTime Expires { get; set; }
}

