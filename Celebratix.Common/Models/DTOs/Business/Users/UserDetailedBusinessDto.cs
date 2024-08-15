using Celebratix.Common.Models.DbModels;
using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.Business.Users;

public class UserDetailedBusinessDto
{
    public UserDetailedBusinessDto(ApplicationUser user, ICollection<string> roles)
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
