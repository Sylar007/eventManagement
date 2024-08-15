using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Admin.Users;

public class UserBasicAdminDto
{
    public UserBasicAdminDto(ApplicationUser user)
    {
        Id = user.Id;
        Email = user.Email;
        FullName = user.FullName;
        PhoneNumber = user.PhoneNumber;
        EmailConfirmed = user.EmailConfirmed;
    }

    public string Id { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }
}
