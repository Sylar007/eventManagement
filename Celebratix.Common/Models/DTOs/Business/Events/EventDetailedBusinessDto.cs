using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.Business.Users;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class EventDetailedBusinessDto : EventBasicBusinessDto
{
    /// <summary>
    /// Required includes:
    /// Category
    /// Image
    /// Creator
    /// Currency
    /// TicketTypes
    /// </summary>
    public EventDetailedBusinessDto(Event dbModel, AmountDto revenueDto) : base(dbModel)
    {
        TicketBackgroundImage = dbModel.TicketBackgroundImage != null ? new ImageDto(dbModel.TicketBackgroundImage) : null;
        TicketBackgroundColor = dbModel.TicketBackgroundColor;
        TicketTextColor = dbModel.TicketTextColor;
        OpenDate = dbModel.OpenDate;
        Creator = dbModel.Creator != null ? new UserBasicBusinessDto(dbModel.Creator) : null;
        TicketTypes = dbModel.TicketTypes?.OrderBy(t => t.SortIndex)?.Select(tt => new EventTicketTypeBusinessDto(tt)).ToArray();
        TicketsCheckedIn = dbModel.TicketsCheckedIn ?? 0;
        Revenue = revenueDto;
        Code = dbModel.Code;
    }

    public ImageDto? TicketBackgroundImage { get; set; }

    /// <summary>
    /// Color in hex color code format
    /// </summary>
    public string? TicketBackgroundColor { get; set; }

    /// <summary>
    /// Color in hex color code format
    /// </summary>
    public string? TicketTextColor { get; set; }

    public DateTimeOffset OpenDate { get; set; }

    public UserBasicBusinessDto? Creator { get; set; }

    public ICollection<EventTicketTypeBusinessDto>? TicketTypes { get; set; }

    public int TicketsCheckedIn { get; set; }

    public AmountDto Revenue { get; set; }

    public string? Code { get; set; }
}
