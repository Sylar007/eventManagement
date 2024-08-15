using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs;

public class ChangeEmailDto
{
    [EmailAddress]
    public string NewEmail { get; set; } = null!;
}
