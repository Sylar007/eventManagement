using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs
{
    /// <summary>
    /// Used for requesting a password change
    /// </summary>
    public class ForgotPasswordDto
    {
        [EmailAddress, MaxLength(256)]
        public string Email { get; set; } = null!;
    }
}
