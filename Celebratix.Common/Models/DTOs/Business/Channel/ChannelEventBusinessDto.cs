using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Channel;

public sealed class ChannelEventBusinessDto
{
    public ChannelEventBusinessDto(ChannelEvent channelEvent)
    {
        Id = channelEvent.Id;
        ChannelId = channelEvent.ChannelId;
        EventId = channelEvent.EventId;
        ResaleEnabled = channelEvent.ResaleEnabled;
        ResaleDisabledDescription = channelEvent.ResaleDisabledDescription;
        ResaleRedirectUrl = channelEvent.ResaleRedirectUrl;
    }

    public Guid Id { get; set; }
    public Guid ChannelId { get; set; }
    public int EventId { get; set; }
    public bool ResaleEnabled { get; set; } = false;
    public string? ResaleDisabledDescription { get; set; }
    public Uri? ResaleRedirectUrl { get; set; }
}