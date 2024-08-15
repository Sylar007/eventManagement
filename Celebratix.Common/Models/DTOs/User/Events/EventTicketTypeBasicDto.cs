namespace Celebratix.Common.Models.DTOs.User.Events;

public class EventTicketTypeBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Selected { get; set; }
}