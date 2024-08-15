using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class EventTicketTypeTicketStatsBusinessDto
{
    public EventTicketTypeTicketStatsBusinessDto(EventTicketType dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
        TicketsSold = dbModel.TicketsSold;
        TicketsCheckedIn = dbModel.TicketsCheckedIn;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public int TicketsSold { get; set; }

    public int TicketsCheckedIn { get; set; }
}
