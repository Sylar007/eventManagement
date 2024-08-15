using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User.Events;

public class EventDetailedDto : EventBasicDto
{
    /// <summary>
    /// Required includes:
    /// TicketTypes
    /// Business
    /// Currency
    /// Category
    /// Image
    /// </summary>
    public EventDetailedDto(Event dbModel) : base(dbModel)
    {
        Description = dbModel.Description;
        Website = dbModel.Website;
        TicketTypes = dbModel.TicketTypes?.OrderBy(t => t.SortIndex)?.Select(tt => new EventTicketTypeDto(tt)).ToArray();
        Currency = new CurrencyDto(dbModel.Currency!);
        TicketBackgroundImage = dbModel.TicketBackgroundImage != null ? new ImageDto(dbModel.TicketBackgroundImage) : null;
        TicketBackgroundColor = dbModel.TicketBackgroundColor;
        TicketTextColor = dbModel.TicketTextColor;
    }

    public string? Description { get; set; }

    public CurrencyDto Currency { get; set; }

    public ImageDto? TicketBackgroundImage { get; set; }

    /// <summary>
    /// Color in hex color code format
    /// </summary>
    public string? TicketBackgroundColor { get; set; }

    /// <summary>
    /// Color in hex color code format
    /// </summary>
    public string? TicketTextColor { get; set; }

    public ICollection<EventTicketTypeDto>? TicketTypes { get; set; }
}
