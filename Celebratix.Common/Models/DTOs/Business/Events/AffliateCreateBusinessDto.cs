using Celebratix.Common.SwaggerFilters;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class AffliateCreateBusinessDto
{
    public Guid ChannelId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    [SwaggerOptional]
    public string? CustomCode { get; set; }
}
