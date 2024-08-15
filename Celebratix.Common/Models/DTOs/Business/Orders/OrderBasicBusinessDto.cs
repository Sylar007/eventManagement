using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Models.DTOs.Business.Users;

namespace Celebratix.Common.Models.DTOs.Business.Orders;

public class OrderBasicBusinessDto
{
    /// <summary>
    /// Required includes:
    /// Currency
    /// Purchaser
    /// AffiliateCode
    /// AffiliateCode.Orders
    /// AffiliateCode.Events
    /// </summary>
    public OrderBasicBusinessDto(Order dbModel)
    {
        Id = dbModel.Id;
        Type = dbModel.Type;
        OrderStatus = dbModel.Status;
        Purchaser = dbModel.Purchaser != null ? new UserBasicBusinessDto(dbModel.Purchaser) : null;
        TicketType = dbModel.TicketType?.Name!;
        Event = dbModel.TicketType?.Event?.Name!;
        AffiliateCode = dbModel.AffiliateCode != null ? new AffliateBusinessDto(dbModel.AffiliateCode) : null;
        TicketQuantity = dbModel.TicketQuantity;
        Amount = new AmountDto(dbModel.BaseAmount, dbModel.ServiceAmount, dbModel.ApplicationAmount);
        VatAmount = dbModel.VatAmount;
        Currency = dbModel.Currency != null ? new CurrencyDto(dbModel.Currency) : null;
        CreatedAt = dbModel.CreatedAt;
        CompletedAt = dbModel.CompletedAt;
    }

    public Guid Id { get; set; }

    public Enums.OrderType Type { get; set; }

    public Enums.OrderStatus OrderStatus { get; set; }

    public UserBasicBusinessDto? Purchaser { get; set; }

    /// <summary>
    /// Name of ticket type
    /// </summary>
    public string TicketType { get; set; }

    /// <summary>
    /// Title of Event
    /// </summary>
    public string Event { get; set; }

    public AffliateBusinessDto? AffiliateCode { get; set; }

    public int TicketQuantity { get; set; }

    public AmountDto Amount { get; set; }

    public decimal VatAmount { get; set; }

    public CurrencyDto? Currency { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }
}
