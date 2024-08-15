namespace Celebratix.Common.Models.DTOs.Business.Events
{
    public class EventChannelTicketTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public bool Selected { get; set; }
    }
}
