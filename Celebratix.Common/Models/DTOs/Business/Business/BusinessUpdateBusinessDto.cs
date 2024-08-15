using System.ComponentModel.DataAnnotations;

namespace Celebratix.Common.Models.DTOs.Business.Business;

public class BusinessUpdateBusinessDto
{
    [MaxLength(1000)]
    public string? FacebookPixelId { get; set; }
    [MaxLength(1000), Url]
    public string? EventsPageUrl { get; set; }
}
