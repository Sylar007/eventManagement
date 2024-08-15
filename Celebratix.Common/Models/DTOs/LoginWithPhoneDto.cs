using System.ComponentModel.DataAnnotations;
using Celebratix.Common.SwaggerFilters;

namespace Celebratix.Common.Models.DTOs
{
    public class LoginWithPhoneDto
    {
        [Phone, MaxLength(100)]
        public string Phone { get; set; } = null!;

        public string VerificationCode { get; set; } = null!;

        [SwaggerOptional]
        public bool StaySignedIn { get; set; } = false; // I.e. IsPersistent
    }
}
