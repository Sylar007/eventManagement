using Celebratix.Common.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Celebratix.Common.Models.DTOs.Business;
[ModelBinder(BinderType = typeof(MetadataValueModelBinder))]
public class ChannelTicketsDto
{
    public string[] EventTicketIds { get; set; } = default!;
    public string? Group { get; set; } = string.Empty;
    public bool AddDescription { get; set; } = false;
    public bool OpenByDefault { get; set; } = false;
}
