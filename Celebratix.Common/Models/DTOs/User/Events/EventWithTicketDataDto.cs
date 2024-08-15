using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User.Events;

public class EventWithTicketDataDto : EventBasicDto
{
    /// <summary>
    /// Required includes:
    /// Category
    /// Image
    /// </summary>
    public EventWithTicketDataDto(Event dbModel, int ticketCount) : base(dbModel)
    {
        TicketCount = ticketCount;
    }

    public int TicketCount { get; set; }
}
