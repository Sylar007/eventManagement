namespace Celebratix.Common.Models.DTOs.Business.Channel;

public sealed class ChannelEventUpdateResaleRequest
{
    public IReadOnlyCollection<ChannelUpdateResaleRequest> ChannelUpdateResaleRequests { get; set; }
        = new List<ChannelUpdateResaleRequest>();
}