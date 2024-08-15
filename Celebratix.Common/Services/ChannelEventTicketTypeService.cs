using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Celebratix.Common.Services
{
    public class ChannelEventTicketTypeService
    {
        private readonly CelebratixDbContext _dbContext;

        public ChannelEventTicketTypeService(CelebratixDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<EventTicketTypeBasicDto>>> GetTicketTypesAsync(Guid channelId, int eventId) =>
            await _dbContext.EventTicketTypes
                .Where(tt => tt.EventId == eventId)
                .OrderBy(ett => ett.Name)
                .Select(ett =>
                    new EventTicketTypeBasicDto
                    {
                        Id = ett.Id,
                        Name = ett.Name,
                        Selected = _dbContext.ChannelEventTicketTypes
                            .Include(cett => cett.ChannelEvent)
                            .Any(cett => cett.EventTicketTypeId == ett.Id && cett.ChannelEvent.ChannelId == channelId)
                    })
                .ToListAsync();

        public async Task<Result<List<EventTicketTypeBasicDto>>> AddOrUpdateAsync(Guid channelId, int eventId, List<Guid> selectedTicketTypes)
        {
            if (!selectedTicketTypes.Any())
            {
                return Result.Fail(
                    new CelebratixError(ErrorCode.GenericValidator, "There isn't any valid ticket type."));
            }

            var existingTicketTypes = await _dbContext.ChannelEventTicketTypes
                .Include(cett => cett.ChannelEvent)
                .Where(ectt => ectt.ChannelEvent.Event.Id == eventId
                               && ectt.ChannelEvent.Channel.Id == channelId)
                .ToListAsync();

            _dbContext.ChannelEventTicketTypes.RemoveRange(existingTicketTypes);

            var channelEvent = await _dbContext.ChannelEvents
                .Where(e => e.EventId == eventId && e.ChannelId == channelId)
                .SingleOrDefaultAsync();

            if (channelEvent == null)
            {
                var channelEntity = await _dbContext.Channels.SingleOrDefaultAsync(c => c.Id == channelId);
                var eventEntity = await _dbContext.Events.SingleOrDefaultAsync(e => e.Id == eventId);

                if (channelEntity == null || eventEntity == null)
                {
                    return Result.Fail(new CelebratixError(ErrorCode.GenericValidator,
                        $"Channel {channelId} and/or Event {eventId} could not be found."));
                }

                channelEvent = new()
                {
                    Channel = channelEntity,
                    Event = eventEntity
                };

                _dbContext.ChannelEvents.Add(channelEvent);
            }

            foreach (var ticketTypeId in selectedTicketTypes)
            {
                var ticketType = await _dbContext.EventTicketTypes
                    .Where(ett => ett.Id == ticketTypeId
                                  && ett.EventId == eventId)
                    .SingleOrDefaultAsync();

                if (ticketType == null)
                {
                    return Result.Fail(new CelebratixError(ErrorCode.GenericValidator,
                        $"TicketTypeId {ticketTypeId} is not valid or it does not have relationship with EventId {eventId}"));
                }

                _dbContext.Add(new ChannelEventTicketType
                {
                    EventTicketTypeId = ticketTypeId,
                    ChannelEventId = channelEvent.Id
                });
            }

            await _dbContext.SaveChangesAsync();

            return await GetTicketTypesAsync(channelId, eventId);
        }
    }
}
