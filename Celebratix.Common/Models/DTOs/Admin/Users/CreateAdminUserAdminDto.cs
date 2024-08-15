namespace Celebratix.Common.Models.DTOs.Admin.Users;

public class CreateAdminUserAdminDto : CreateUserDto
{
    public string CurrentPassword { get; set; } = null!;

    public Enums.Role AdminRole { get; set; }
}
