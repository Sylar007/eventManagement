using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.Extensions;

public static class ChannelEventExtensions
{
    public static ChannelEvent SetChannel(this ChannelEvent channelEvent,
        Channel channel)
    {
        channelEvent.ChannelId = channel.Id;

        return channelEvent;
    }

    public static ChannelEvent SetEvent(this ChannelEvent channelEvent,
        Event @event)
    {
        channelEvent.EventId = @event.Id;

        return channelEvent;
    }
}