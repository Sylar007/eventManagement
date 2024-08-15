using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs;

public class UpdateAccountEmailDto
{
    [EmailAddress, MaxLength(256)]
    public string Email { get; set; } = null!;
}
