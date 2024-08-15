using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class EventDetailedDemographicsBusinessDto
{

    public EventDetailedDemographicsBusinessDto(List<Ticket> tickets)
    {
        foreach (var ticket in tickets)
        {
            if (ticket.Owner == null) continue;
            switch (ticket.Owner.Gender)
            {
                case Enums.Gender.Male: Male++; break;
                case Enums.Gender.Female: Female++; break;
                case Enums.Gender.Other: Other++; break;
            }
            Total++;
        }
        if (Total != 0)
        {
            MaleFraction = Male / (decimal)Total;
            FemaleFraction = Female / (decimal)Total;
            OtherFraction = Other / (decimal)Total;
        }
    }

    public int Male { get; set; }
    public int Female { get; set; }
    public int Other { get; set; }

    public int Total { get; set; }

    public decimal MaleFraction { get; set; }
    public decimal FemaleFraction { get; set; }
    public decimal OtherFraction { get; set; }
}

public class EventTicketStatsBusinessDto
{
    public EventTicketStatsBusinessDto(Event dbModel, List<Ticket> tickets)
    {
        TotalSoldTickets = dbModel.TicketsSold;
        TotalCheckedInTickets = dbModel.TicketsCheckedIn;
        TicketTypes = dbModel.TicketTypes?.OrderBy(t => t.SortIndex)?.Select(t => new EventTicketTypeTicketStatsBusinessDto(t)).ToArray();
        Demographics = new EventDetailedDemographicsBusinessDto(tickets);
    }

    public int? TotalSoldTickets { get; set; }

    public int? TotalCheckedInTickets { get; set; }

    public ICollection<EventTicketTypeTicketStatsBusinessDto>? TicketTypes { get; set; }

    public EventDetailedDemographicsBusinessDto Demographics { get; set; }
}
