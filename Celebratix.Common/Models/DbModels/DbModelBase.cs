namespace Celebratix.Common.Models.DbModels;

public abstract class DbModelBase
{
    /// <summary>
    /// Time the entity was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last time the entity was updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
