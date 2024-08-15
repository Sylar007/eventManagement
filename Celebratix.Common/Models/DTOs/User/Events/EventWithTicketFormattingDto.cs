using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User.Events;

public class EventWithTicketFormattingDto : EventBasicDto
{
    /// <summary>
    /// Required includes:
    /// Category
    /// Image
    /// TicketBackgroundImage
    /// </summary>
    public EventWithTicketFormattingDto(Event dbModel) : base(dbModel)
    {
        TicketBackgroundImage = dbModel.TicketBackgroundImage != null ? new ImageDto(dbModel.TicketBackgroundImage) : null;
        TicketBackgroundColor = dbModel.TicketBackgroundColor;
        TicketTextColor = dbModel.TicketTextColor;
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
}
