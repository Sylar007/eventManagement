using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User.Events;

public class ChannelEventTicketTypeBasicDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public bool Selected => true;

    public static explicit operator ChannelEventTicketTypeBasicDto(
        EventTicketType eventTicketType) =>
        new()
        {
            Id = eventTicketType.Id,
            Name = eventTicketType.Name
        };
}