using AutoFixture;
using AutoFixture.Kernel;
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Tests.Extensions
{
    public static class FixtureExtensions
    {
        public static ChannelEvent CreateChannelEvent(
            this ISpecimenBuilder fixture,
            Channel channel, Event @event) =>
            fixture.Create<ChannelEvent>()
                .SetChannel(channel)
                .SetEvent(@event);

        public static IEnumerable<EventTicketType> CreateEventTicketTypes(
            this ISpecimenBuilder fixture,
            Event @event, int count) =>
            fixture
                .CreateMany<EventTicketType>(count)
                .SetEvent(@event);

        public static Channel CreateChannel(
            this ISpecimenBuilder fixture,
            ApplicationUser user) =>
            fixture
                .Create<Channel>()
                .SetBusiness(user.BusinessId);

        public static Event CreateEvent(
            this ISpecimenBuilder fixture, ApplicationUser user) =>
            fixture
                .Create<Event>()
                .SetCreator(user)
                .SetBusiness(user.BusinessId);

        public static IEnumerable<ChannelEventTicketType> SetChannelEventTicketTypes(
            this ISpecimenBuilder fixture,
            ChannelEvent eventChannel,
            IEnumerable<EventTicketType> ticketTypes)
        {
            return ticketTypes.Select(type => fixture
                .Create<ChannelEventTicketType>()
                .SetEventTicketType(eventChannel, type));
        }
    }
}
