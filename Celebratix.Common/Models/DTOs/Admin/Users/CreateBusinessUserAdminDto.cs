namespace Celebratix.Common.Models.DTOs.Admin.Users;

public class CreateBusinessUserAdminDto : CreateUserDto
{
    public Guid BusinessId { get; set; }

    public Enums.Role Role { get; set; }
}
