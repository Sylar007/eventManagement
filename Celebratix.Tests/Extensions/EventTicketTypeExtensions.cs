using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.Extensions;

public static class EventTicketTypeExtensions
{
    public static EventTicketType SetEvent(
        this EventTicketType ticketType,
        Event @event)
    {
        ticketType.EventId = @event.Id;

        return ticketType;
    }

    public static IEnumerable<EventTicketType> SetEvent(
        this IEnumerable<EventTicketType> ticketType,
        Event @event)
    {
        return ticketType.Select(type =>
            type.SetEvent(@event));
    }
}