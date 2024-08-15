using Celebratix.Common.Models.DTOs.User.Events;

namespace Celebratix.Common.Models.DTOs.Business.Channel;

public class ChannelDetailedDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public Enums.ChannelTemplateTypes TemplateType { get; set; }
    public string? Color { get; init; }

    public IReadOnlyList<ChannelEventTicketTypeBasicDto> SelectedTicketTypes { get; init; } =
        new List<ChannelEventTicketTypeBasicDto>();

    public static explicit operator ChannelDetailedDto(DbModels.Channel channel) =>
        new()
        {
            Id = channel.Id,
            Name = channel.Name,
            Slug = channel.Slug,
            TemplateType = channel.TemplateType,
            Color = channel.Color,
            SelectedTicketTypes = channel.ChannelEvents
                .SelectMany(channelEvent =>
                    channelEvent.SelectedTicketTypes)
                .Select(ticketType =>
                    (ChannelEventTicketTypeBasicDto)ticketType.EventTicketType)
                .ToList()
        };
}