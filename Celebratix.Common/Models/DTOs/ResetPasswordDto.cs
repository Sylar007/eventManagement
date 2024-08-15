using System.ComponentModel.DataAnnotations;
using Celebratix.Common.SwaggerFilters;

namespace Celebratix.Common.Models.DTOs
{
    public class ResetPasswordDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string Token { get; set; } = null!;

        [MinLength(6, ErrorMessage = "min-length#6"), MaxLength(256, ErrorMessage = "max-length#256"), DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [SwaggerOptional]
        public bool StaySignedIn { get; set; } = false; // I.e. IsPersistent
    }
}
