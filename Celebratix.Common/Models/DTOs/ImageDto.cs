using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs;

public class ImageDto
{
    public ImageDto(ImageDbModel image)
    {
        Path = image.Path;
    }

    public string Path { get; set; }
}
