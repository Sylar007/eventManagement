namespace Celebratix.Common.Models.DTOs.Business.Users;

public class CreateBusinessUserDto : CreateUserDto
{
    public Enums.Role Role { get; set; }
}
