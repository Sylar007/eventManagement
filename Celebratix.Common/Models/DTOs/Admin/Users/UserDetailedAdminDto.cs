using System.ComponentModel.DataAnnotations;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Admin.Users;

public class UserDetailedAdminDto
{
    public UserDetailedAdminDto(ApplicationUser user, ICollection<string> roles)
    {
        Id = user.Id;
        Roles = roles;
        Email = user.Email;
        EmailConfirmed = user.EmailConfirmed;
        FullName = user.FullName;
        PhoneNumber = user.PhoneNumber;
        Gender = user.Gender;
        DateOfBirth = user.DateOfBirth;
    }

    public string Id { get; set; }

    public ICollection<string> Roles { get; set; }

    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public Enums.Gender? Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
}
