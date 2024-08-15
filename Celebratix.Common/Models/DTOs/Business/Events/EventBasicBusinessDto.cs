using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.Business.Channel;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class EventBasicBusinessDto
{
    /// <summary>
    /// Required includes:
    /// Category
    /// Image
    /// Currency
    /// TicketTypes
    /// </summary>
    public EventBasicBusinessDto(Event dbModel)
    {
        Id = dbModel.Id;
        CustomSlug = dbModel.CustomSlug;
        Visible = dbModel.Visible;
        Name = dbModel.Name;
        TicketsSold = dbModel.TicketsSold ?? 0;
        AgeLimit = dbModel.AgeLimit;
        StartDate = dbModel.StartDate;
        EndDate = dbModel.EndDate;
        Location = dbModel.Location;
        City = dbModel.City;
        Category = dbModel.Category != null ? new CategoryDto(dbModel.Category) : null;
        Status = dbModel.Status.ToString();
        Image = dbModel.Image != null ? new ImageDto(dbModel.Image) : null;
        Currency = dbModel.Currency != null ? new CurrencyDto(dbModel.Currency) : default!;
        MaxTicketsAvailable = dbModel.MaxTicketsAvailable ?? 0;
        CreatedAt = dbModel.CreatedAt;
        Channels = dbModel.Channels.Select(c => new ChannelBusinessDto(c)).ToList();
        ExternalEventUrl = dbModel.ExternalEventUrl;
        Description = dbModel.Description;
        Website = dbModel.Website;
        Publish = dbModel.Publish;
    }

    public int Id { get; set; }

    /// <summary>
    /// Can be used instead of the id to fetch the event
    /// </summary>
    public string? CustomSlug { get; set; }

    public string Name { get; set; }

    public int TicketsSold { get; set; }

    public int? AgeLimit { get; set; }

    public bool Visible { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }
    public string? Status { get; set; }

    public CategoryDto? Category { get; set; }

    public string? Location { get; set; }

    public string? City { get; set; }

    public ImageDto? Image { get; set; }

    public CurrencyDto Currency { get; set; }

    public int MaxTicketsAvailable { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public List<ChannelBusinessDto> Channels { get; set; }

    public string? ExternalEventUrl { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public bool Publish { get; set; }
}
