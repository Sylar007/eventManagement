using Microsoft.AspNetCore.Http;

namespace Celebratix.Common.Models.DTOs.Business.Events
{
    public class EventCreateRequest
    {
        public string? CustomSlug { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public string? Location { get; set; }

        public string? City { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? Postcode { get; set; }

        public string? Website { get; set; }

        public string CurrencyId { get; set; } = null!;
    }
}
