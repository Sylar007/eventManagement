using AutoFixture;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Models;
using Celebratix.Tests.Extensions;
using Celebratix.Tests.Fixtures;
using Celebratix.Tests.SpecimenBuilders;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Celebratix.Tests.Controllers.V2
{
    public sealed class ChannelEventTicketTypesControllerTests :
        IClassFixture<ContainerizedFixture>,
        IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseContextFixture _databaseContext;
        private readonly Fixture _fixture;
        private readonly ApplicationUser _user;

        public ChannelEventTicketTypesControllerTests(ContainerizedFixture factory)
        {
            _httpClient = factory.CreateClient();

            _databaseContext = new DatabaseContextFixture(factory.Services);

            _fixture = new Fixture();
            _fixture.AddEntitySpecimenBuilders();

            _user = _fixture.Create<ApplicationUser>();
            _databaseContext.With(_user);
        }

        [Fact]
        public async Task Gets_event_ticket_types()
        {
            // Arrange
            var @event = _fixture.CreateEvent(_user);
            _databaseContext.With(@event);

            var channel = _fixture.CreateChannel(_user);
            _databaseContext.With(channel);

            var channelEvent = _fixture.CreateChannelEvent(channel, @event);
            _databaseContext.With(channelEvent);

            var ticketTypes = _fixture.CreateEventTicketTypes(@event, 10).ToArray();
            _databaseContext
                .With(ticketTypes.ToArray());

            _databaseContext.EnsureWith(t => t.Id, _fixture.SetChannelEventTicketTypes(
                channelEvent, ticketTypes)
                .ToArray());

            // Act
            using var response = await _httpClient
                .SetUserClaims(_user, Enums.Role.Business)
                .GetAsync($"v2/business/channels/{channel.Id}/events/{@event.Id}/ticket-types");

            // Assert
            response.StatusCode.Should()
                .Be(HttpStatusCode.OK);

            var result = await response.Content
                .DeserializeResponseContent<OperationResult<IEnumerable<EventTicketTypeBasicDto>>>();

            result.Should()
                .NotBeNull();

            result!.IsSuccess.Should()
                .BeTrue();

            result.Data.Should()
                .NotBeNull();

            result.Data!.Count()
                .Should()
                .Be(ticketTypes.Length);
        }

        [Fact]
        public async Task Creates_or_updates_selected_event_ticket_types()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _databaseContext.With(category);

            var @event = _fixture.CreateEvent(_user);
            _databaseContext.With(@event);

            var channel = _fixture.CreateChannel(_user);
            _databaseContext.With(channel);

            var channelEvent = _fixture.CreateChannelEvent(channel, @event);
            _databaseContext.With(channelEvent);

            var ticketTypes = _fixture.CreateEventTicketTypes(@event, 10).ToArray();
            _databaseContext
                .With(ticketTypes)
                .EnsureWith(t => t.Id, _fixture.SetChannelEventTicketTypes(
                    channelEvent,
                    ticketTypes.Take(5)).ToArray());

            // Act
            using var response = await _httpClient
                .SetUserClaims(_user, Enums.Role.Business)
                .PostAsJsonAsync($"v2/business/channels/{channel.Id}/events/{@event.Id}/ticket-types",
                    ticketTypes.Skip(9).Select(type => type.Id));

            // Assert
            response.StatusCode.Should()
                .Be(HttpStatusCode.OK);

            var result = await response.Content
                .DeserializeResponseContent<OperationResult<IEnumerable<EventTicketTypeBasicDto>>>();

            result.Should()
                .NotBeNull();

            result!.IsSuccess.Should()
                .BeTrue();

            result.Data.Should()
                .NotBeNull();

            result.Data!.Count()
                .Should()
                .Be(10);

            _databaseContext.HasOne<ChannelEventTicketType>(type =>
                type.ChannelEventId == channelEvent.Id);
        }

        [Fact]
        public async Task Returns_400_when_no_tickets_types_specified()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _databaseContext.With(category);

            var @event = _fixture.CreateEvent(_user);
            _databaseContext.With(@event);

            var channel = _fixture.CreateChannel(_user);
            _databaseContext.With(channel);

            var channelEvent = _fixture.CreateChannelEvent(channel, @event);
            _databaseContext.With(channelEvent);

            var ticketTypes = _fixture.CreateEventTicketTypes(@event, 10).ToArray();
            _databaseContext
                .With(ticketTypes)
                .EnsureWith(t => t.Id, _fixture.SetChannelEventTicketTypes(
                    channelEvent,
                    ticketTypes.Take(5)).ToArray());

            // Act
            using var response = await _httpClient
                .SetUserClaims(_user, Enums.Role.Business)
                .PostAsJsonAsync($"v2/business/channels/{channel.Id}/events/{@event.Id}/ticket-types",
                    Array.Empty<Guid>());

            // Assert
            response.StatusCode.Should()
                .Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Returns_400_when_channel_does_not_exist()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _databaseContext.With(category);

            var @event = _fixture.CreateEvent(_user);
            _databaseContext.With(@event);

            var channel = _fixture.CreateChannel(_user);
            _databaseContext.With(channel);

            var channelEvent = _fixture.CreateChannelEvent(channel, @event);
            _databaseContext.With(channelEvent);

            var ticketTypes = _fixture.CreateEventTicketTypes(@event, 10).ToArray();
            _databaseContext
                .With(ticketTypes)
                .EnsureWith(t => t.Id, _fixture.SetChannelEventTicketTypes(
                    channelEvent,
                    ticketTypes.Take(5)).ToArray());

            // Act
            using var response = await _httpClient
                .SetUserClaims(_user, Enums.Role.Business)
                .PostAsJsonAsync($"v2/business/channels/{Guid.NewGuid()}/events/{@event.Id}/ticket-types",
                    Array.Empty<Guid>());

            // Assert
            response.StatusCode.Should()
                .Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Returns_404_when_event_does_not_exist()
        {
            // Arrange
            var category = _fixture.Create<Category>();
            _databaseContext.With(category);

            var @event = _fixture.CreateEvent(_user);
            _databaseContext.With(@event);

            var channel = _fixture.CreateChannel(_user);
            _databaseContext.With(channel);

            var channelEvent = _fixture.CreateChannelEvent(channel, @event);
            _databaseContext.With(channelEvent);

            var ticketTypes = _fixture.CreateEventTicketTypes(@event, 10).ToArray();
            _databaseContext
                .With(ticketTypes)
                .EnsureWith(t => t.Id, _fixture.SetChannelEventTicketTypes(
                    channelEvent,
                    ticketTypes.Take(5)).ToArray());

            // Act
            using var response = await _httpClient
                .SetUserClaims(_user, Enums.Role.Business)
                .PostAsJsonAsync($"v2/business/channels/{channel.Id}/events/{Guid.NewGuid()}/ticket-types",
                    Array.Empty<Guid>());

            // Assert
            response.StatusCode.Should()
                .Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Creates_channel_event_when_missing()
        {
            // Arrange
            var @event = _fixture.CreateEvent(_user);
            _databaseContext.With(@event);

            var channel = _fixture.CreateChannel(_user);
            _databaseContext.With(channel);

            var ticketTypes = _fixture.CreateEventTicketTypes(@event, 10).ToArray();
            _databaseContext.With(ticketTypes);

            // Act
            using var response = await _httpClient
                .SetUserClaims(_user, Enums.Role.Business)
                .PostAsJsonAsync($"v2/business/channels/{channel.Id}/events/{@event.Id}/ticket-types",
                    ticketTypes.Skip(9).Select(type => type.Id));

            // Assert
            response.StatusCode.Should()
                .Be(HttpStatusCode.OK);

            var result = await response.Content
                .DeserializeResponseContent<OperationResult<IEnumerable<EventTicketTypeBasicDto>>>();

            result.Should()
                .NotBeNull();

            result!.IsSuccess.Should()
                .BeTrue();

            result.Data.Should()
                .NotBeNull();

            result.Data!.Count()
                .Should()
                .Be(10);

            _databaseContext.HasOne<ChannelEvent>(channelEvent =>
                channelEvent.EventId == @event.Id);
        }

        public void Dispose() =>
            _httpClient.Dispose();
    }
}
