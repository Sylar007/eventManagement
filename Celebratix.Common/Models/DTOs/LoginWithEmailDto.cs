using System.ComponentModel.DataAnnotations;
using Celebratix.Common.SwaggerFilters;

namespace Celebratix.Common.Models.DTOs
{
    public class LoginWithEmailDto
    {
        [EmailAddress, MaxLength(256)]
        public string Email { get; set; } = null!;

        [MinLength(6), MaxLength(256), DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [SwaggerOptional]
        public bool StaySignedIn { get; set; } = false; // I.e. IsPersistent
    }
}
