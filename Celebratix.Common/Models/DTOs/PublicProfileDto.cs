using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User;

public class PublicProfileDto
{
    public PublicProfileDto(ApplicationUser user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        FullName = user.FullName;
    }

    public string Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? FullName { get; set; }
}
