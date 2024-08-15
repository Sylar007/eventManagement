using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Models.DTOs.User.Orders;

namespace Celebratix.Common.Models.DTOs.User.TicketTransfer;

public class TicketTransferOfferDto
{
    /// <summary>
    /// Required includes:
    /// Currency
    /// Ticket.TicketType.Event.Category + Image
    /// </summary>
    public TicketTransferOfferDto(TicketTransferOffer dbModel)
    {
        Id = dbModel.Id;
        Event = new EventBasicDto(dbModel.Ticket!.TicketType!.Event!);
        TicketTypeId = dbModel.Ticket.TicketTypeId;
        TicketTypeName = dbModel.Ticket.TicketType.Name;
        Transferred = dbModel.Transferred;
        Cancelled = dbModel.Cancelled;
        Available = dbModel.Available;
        Code = dbModel.Code;
        FulfilledByOrder = dbModel.FulfilledByOrder != null ? new OrderBasicDto(dbModel.FulfilledByOrder) : null;
    }

    public Guid Id { get; set; }

    public EventBasicDto Event { get; set; }

    public Guid TicketTypeId { get; set; }

    public string TicketTypeName { get; set; }

    public bool Transferred { get; set; }

    public bool Available { get; set; }

    public string Code { get; set; }

    public bool Cancelled { get; set; }

    public OrderBasicDto? FulfilledByOrder { get; set; }
}
