using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Models.DTOs.User.Tickets;

namespace Celebratix.Common.Models.DTOs.User.Orders;

public class OrderFromMagicDto
{
    /// <summary>
    /// Required includes:
    /// order.Event.Business
    /// order.Event.Image
    /// order.Event.TicketBackgroundImage
    /// order.Event.Category
    /// </summary>
    /// <param name="dbModel"></param>
    public OrderFromMagicDto(Order order, List<TicketQrDto>? tickets)
    {
        Event = new EventWithTicketFormattingDto(order.Event ?? order.TicketType!.Event!);
        Tickets = tickets;
        Status = order.Status;
    }

    public EventWithTicketFormattingDto Event { get; set; }
    public List<TicketQrDto>? Tickets { get; set; }
    public Enums.OrderStatus Status { get; set; }
}
