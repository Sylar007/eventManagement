namespace Celebratix.Common.Models.DbModels;

/// <summary>
/// Public images with permanent url
/// </summary>
public class ImageDbModel : FileDbModel
{
    /// <summary>
    /// A permanent link to the file on the storage
    /// E.g. link to Azure blob storage or locale path
    ///
    /// Normally not to be used
    /// </summary>
    public string PermaLink { get; set; } = null!;

    /// <summary>
    /// Image path, to be used for fetching the image normally
    /// </summary>
    public string Path { get; set; } = null!;
}
