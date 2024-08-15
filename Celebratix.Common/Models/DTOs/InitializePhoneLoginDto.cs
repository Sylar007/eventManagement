using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs;

public class InitializePhoneLoginDto
{
    [Phone, MaxLength(100)]
    public string Phone { get; set; } = null!;
}
