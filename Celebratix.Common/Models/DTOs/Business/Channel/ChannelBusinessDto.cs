using Celebratix.Common.Models.DTOs.User.Events;
using Microsoft.AspNetCore.Http;

namespace Celebratix.Common.Models.DTOs.Business.Channel;

public class ChannelBusinessDto
{
    public ChannelBusinessDto()
    {

    }

    public ChannelBusinessDto(DbModels.Channel channel)
    {
        Id = channel.Id;
        Name = channel.Name;
        Slug = channel.Slug;
        TemplateType = channel.TemplateType;
        Color = channel.Color;
    }

    public ChannelBusinessDto(DbModels.ChannelEvent channelEvent)
    {
        Id = channelEvent.ChannelId;
        Name = channelEvent.Channel.Name;
        Slug = channelEvent.Channel.Slug;
        TemplateType = channelEvent.Channel.TemplateType;
        Color = channelEvent.Channel.Color;
        SelectedTicketTypes =
            channelEvent.SelectedTicketTypes
                .Select(ticket =>
                    new EventTicketTypeBasicDto
                    {
                        Id = ticket.EventTicketTypeId,
                        Name = ticket.EventTicketType.Name,
                        Selected = true
                    }).ToList();
    }

    public ChannelBusinessDto(DbModels.Channel channel,
        List<DbModels.EventTicketType> ticketTypes,
        List<DbModels.ChannelEventTicketType> selectedTicketTypes) : this(channel)
    {
        SelectedTicketTypes = ticketTypes
            .Select(t => new EventTicketTypeBasicDto
            {
                Id = t.Id,
                Name = t.Name,
                Selected = selectedTicketTypes.Any(s => s.EventTicketTypeId == t.Id)
            })
            .ToList();
    }


    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public Enums.ChannelTemplateTypes TemplateType { get; set; }
    public string? Color { get; set; }
    public IFormFile? Image { get; set; }
    public List<EventTicketTypeBasicDto> SelectedTicketTypes { get; set; } = new();
}
