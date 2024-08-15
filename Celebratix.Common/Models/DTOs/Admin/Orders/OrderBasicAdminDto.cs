using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Admin.Orders;

public class OrderBasicAdminDto
{
    /// <summary>
    /// Required includes:
    /// Currency
    /// TicketType
    ///     + Event
    /// </summary>
    public OrderBasicAdminDto(Order dbModel)
    {
        Id = dbModel.Id;
        Type = dbModel.Type;
        OrderStatus = dbModel.Status;
        EventTitle = dbModel.TicketType?.Event?.Name!;
        TicketQuantity = dbModel.TicketQuantity;
        Amount = new AmountDto(dbModel.BaseAmount, dbModel.ServiceAmount, dbModel.ApplicationAmount);
        Currency = dbModel.Currency != null ? new CurrencyDto(dbModel.Currency) : null;
        CreatedAt = dbModel.CreatedAt;
    }

    public Guid Id { get; set; }

    public Enums.OrderType Type { get; set; }

    public Enums.OrderStatus OrderStatus { get; set; }

    public string EventTitle { get; set; }

    public int TicketQuantity { get; set; }

    public AmountDto Amount { get; set; }

    public CurrencyDto? Currency { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
