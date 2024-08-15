using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Events;

namespace Celebratix.Common.Models.DTOs.User.Tickets;

public class TicketDto
{
    /// <summary>
    /// Required includes:
    /// Ticket.TicketType.Event.Image
    /// Ticket.TicketType.Event.Category
    /// Ticket.TicketType.Event.Currency
    /// OriginalOrder
    /// </summary>
    public TicketDto(Ticket dbModel, decimal? maxResellPriceRatio)
    {
        var currency = dbModel.TicketType!.Event!.Currency!;

        Id = dbModel.Id;
        TicketTypeName = dbModel.TicketType!.Name;
        Event = new EventBasicDto(dbModel.TicketType!.Event!);
        ResellCurrency = new CurrencyDto(currency);
        if (dbModel.OriginalOrder != null)
            OriginalPrice = dbModel.OriginalOrder.Amount / dbModel.OriginalOrder.TicketQuantity;
        else
            OriginalPrice = 0m;
        MinResellPrice = currency.MinMarketplaceListingPrice;
        if (maxResellPriceRatio != null && MinResellPrice != null)
        {
            MaxResellPrice = Math.Max((decimal)(OriginalPrice * maxResellPriceRatio), MinResellPrice.Value);
        }
    }

    public Guid Id { get; set; }

    public string TicketTypeName { get; set; }

    public EventBasicDto Event { get; set; }

    public CurrencyDto ResellCurrency { get; set; }

    public decimal OriginalPrice { get; set; }

    public decimal? MinResellPrice { get; set; }

    public decimal? MaxResellPrice { get; set; }
}
