using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;

namespace Celebratix.Common.Models.DTOs.User.Orders;

public class OrderBasicDto
{
    /// <summary>
    /// Required includes:
    /// TicketType.Event.Image
    /// TicketType.Event.Category
    /// Currency
    /// </summary>
    public OrderBasicDto(Order dbModel)
    {
        Id = dbModel.Id;
        Type = dbModel.Type;
        Status = dbModel.Status;
        Event = new EventBasicDto(dbModel.TicketType!.Event!);
        TicketQuantity = dbModel.TicketQuantity;
        AmountSplit = new AmountDto(dbModel.BaseAmount, dbModel.ServiceAmount, dbModel.ApplicationAmount);
        Amount = AmountSplit.Base + AmountSplit.ServiceFee + AmountSplit.ApplicationFee;
        Currency = dbModel.Currency != null ? new CurrencyDto(dbModel.Currency) : null;
        CreatedAt = dbModel.CreatedAt;
    }

    public Guid Id { get; set; }
    public Enums.OrderType Type { get; set; }
    public Enums.OrderStatus Status { get; set; }
    public EventBasicDto Event { get; set; }
    public int TicketQuantity { get; set; }
    public decimal Amount { get; set; }
    public AmountDto AmountSplit { get; set; }

    public CurrencyDto? Currency { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
