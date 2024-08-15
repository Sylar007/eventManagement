using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs;

public class CreateUserDto
{
    [EmailAddress(ErrorMessage = "invalid-email"), MaxLength(256, ErrorMessage = "max-length#256")]
    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}
