using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;

namespace Celebratix.Common.Models.DTOs.User.Orders;

public class OrderDetailedDto
{
    /// <summary>
    /// Required includes:
    /// Event.Business
    /// Event.Image
    /// Event.TicketBackgroundImage
    /// Event.Category
    /// Currency
    /// </summary>
    /// <param name="dbModel"></param>
    public OrderDetailedDto(Order dbModel, string magic)
    {
        Id = dbModel.Id;
        Type = dbModel.Type;
        Status = dbModel.Status;
        Event = new EventWithTicketFormattingDto(dbModel.Event ?? dbModel.TicketType!.Event!);
        TicketType = new EventTicketTypeDto(dbModel.TicketType!);
        TicketQuantity = dbModel.TicketQuantity;
        AmountSplit = new AmountDto(dbModel.BaseAmount, dbModel.ServiceAmount, dbModel.ApplicationAmount);
        Amount = AmountSplit.Base + AmountSplit.ServiceFee + AmountSplit.ApplicationFee;
        VatAmount = dbModel.VatAmount;
        Currency = dbModel.Currency != null ? new CurrencyDto(dbModel.Currency) : null;
        CreatedAt = dbModel.CreatedAt;
        CompletedAt = dbModel.CompletedAt;
        Magic = magic;
    }

    public Guid Id { get; set; }
    public Enums.OrderType Type { get; set; }
    public Enums.OrderStatus Status { get; set; }
    public EventWithTicketFormattingDto Event { get; set; }
    public EventTicketTypeDto TicketType { get; set; }
    public int TicketQuantity { get; set; }
    public decimal Amount { get; set; }
    public AmountDto AmountSplit { get; set; }

    public decimal VatAmount { get; set; }

    public CurrencyDto? Currency { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public string Magic { get; set; }
}
