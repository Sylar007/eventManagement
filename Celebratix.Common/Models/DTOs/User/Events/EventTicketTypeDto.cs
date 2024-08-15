using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.User.Events;

public class EventTicketTypeDto
{
    public EventTicketTypeDto(EventTicketType dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
        Price = dbModel.Price;
        // The user doesn't need to know what the application fee is
        ServiceFee = dbModel.ServiceFee + dbModel.ApplicationFee;
        TotalPrice = dbModel.TotalPrice;
        Vat = dbModel.CustomVat;
        AvailableFrom = dbModel.AvailableFrom;
        AvailableUntil = dbModel.AvailableUntil;
        var available = dbModel.AvailableTickets ?? int.MaxValue;
        NumberOfTicketsAvailableForOneOrder = Math.Min(available, dbModel.MaxTicketsPerPurchase);
        MinTicketsPerPurchase = Math.Min(available, dbModel.MinTicketsPerPurchase);
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public decimal ServiceFee { get; set; }

    public decimal TotalPrice { get; set; }

    /// <summary>
    /// As a fraction. E.g. 0.12
    /// </summary>
    public decimal Vat { get; set; }

    public DateTimeOffset? AvailableFrom { get; set; }

    public DateTimeOffset? AvailableUntil { get; set; }

    /// <summary>
    /// Instead of showing the total amount of tickets, which would leak info about the event sales etc.
    /// This amount is used, which at most is the the max amount of tickets a user can buy.
    /// min(ticketsAvailable, maxTicketsPerPurchase)
    /// </summary>
    public int NumberOfTicketsAvailableForOneOrder { get; set; }

    public int MinTicketsPerPurchase { get; set; }
}
