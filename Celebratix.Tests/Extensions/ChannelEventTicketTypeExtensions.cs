using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.Extensions;

public static class ChannelEventTicketTypeExtensions
{
    public static IEnumerable<ChannelEventTicketType> SetChannelEvent(
        this IEnumerable<ChannelEventTicketType> ticketType,
        ChannelEvent channel)
    {
        return ticketType.Select(type =>
            type.SetChannelEvent(channel));
    }

    public static ChannelEventTicketType SetChannelEvent(
        this ChannelEventTicketType ticketType,
        ChannelEvent channel)
    {
        ticketType.ChannelEventId = channel.Id;

        return ticketType;
    }

    public static IEnumerable<ChannelEventTicketType> SetEventTicketType(
        this IEnumerable<ChannelEventTicketType> ticketType,
        ChannelEvent channelEvent, IEnumerable<EventTicketType> eventTicketTypes)
    {
        return ticketType.Select(type =>
            type.SetEventTicketType(channelEvent, eventTicketTypes));
    }

    public static ChannelEventTicketType SetEventTicketType(
        this ChannelEventTicketType channelTicketType,
        ChannelEvent channelEvent, IEnumerable<EventTicketType> eventTicketTypes)
    {
        foreach (var ticketType in eventTicketTypes)
        {
            channelTicketType.EventTicketTypeId = ticketType.Id;
            channelTicketType.ChannelEventId = channelEvent.Id;
        }

        return channelTicketType;
    }

    public static ChannelEventTicketType SetEventTicketType(
        this ChannelEventTicketType channelTicketType,
        ChannelEvent channelEvent, EventTicketType eventTicketType)
    {
        channelTicketType.EventTicketTypeId = eventTicketType.Id;
        channelTicketType.ChannelEventId = channelEvent.Id;

        return channelTicketType;
    }
}