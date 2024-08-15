namespace Celebratix.Common.Models.DTOs.Business.Channel;

public class ChannelEventRequest : ChannelRequest
{
    public List<Guid> SelectedTicketTypes { get; set; } = new();
}
