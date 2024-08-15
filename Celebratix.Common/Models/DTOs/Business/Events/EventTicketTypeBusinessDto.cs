using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class EventTicketTypeBusinessDto
{
    public EventTicketTypeBusinessDto(EventTicketType dbModel)
    {
        Id = dbModel.Id;
        Name = dbModel.Name;
        Price = dbModel.Price;
        ServiceFee = dbModel.ServiceFee;
        ApplicationFee = dbModel.ApplicationFee;
        TotalPrice = dbModel.TotalPrice;
        CustomVat = dbModel.CustomVat;
        AvailableTickets = dbModel.AvailableTickets;
        PubliclyAvailable = dbModel.PubliclyAvailable;
        AvailableFrom = dbModel.AvailableFrom;
        AvailableUntil = dbModel.AvailableUntil;
        OnlyAffiliates = dbModel.OnlyAffiliates;
        LinkCode = dbModel.LinkCode;
        MaxTicketsAvailable = dbModel.MaxTicketsAvailable;
        MaxTicketsPerPurchase = dbModel.MaxTicketsPerPurchase;
        MinTicketsPerPurchase = dbModel.MinTicketsPerPurchase;
        ReservedTickets = dbModel.ReservedTickets;
        TicketsSold = dbModel.TicketsSold;
        TicketsCheckedIn = dbModel.TicketsCheckedIn;
        Revenue = dbModel.Revenue;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public decimal ServiceFee { get; set; }

    public decimal ApplicationFee { get; set; }

    public decimal TotalPrice { get; set; }

    /// <summary>
    /// As a fraction. E.g. 0.12
    /// </summary>
    public decimal? CustomVat { get; set; }

    public int? MaxTicketsAvailable { get; set; }

    public int MinTicketsPerPurchase { get; set; }

    public int MaxTicketsPerPurchase { get; set; }

    public int ReservedTickets { get; set; }

    public int TicketsSold { get; set; }

    public int TicketsCheckedIn { get; set; }

    public AmountDto? Revenue { get; set; }

    public int? AvailableTickets { get; set; }

    public bool PubliclyAvailable { get; set; }

    public DateTimeOffset? AvailableFrom { get; set; }

    public DateTimeOffset? AvailableUntil { get; set; }

    public string? LinkCode { get; set; }

    public bool OnlyAffiliates { get; set; } = false;
}
