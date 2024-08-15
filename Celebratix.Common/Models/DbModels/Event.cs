using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

[Index(nameof(StartDate))]
[Index(nameof(EndDate))]
[Index(nameof(Visible))]
[Index(nameof(CustomSlug), IsUnique = true)]
public class Event : DbModelBase
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Can be used instead of the id to fetch the event
    /// </summary>
    public string? CustomSlug { get; set; }

    public bool Visible { get; set; } = false;

    [ForeignKey(nameof(Business))] // TODO: remove?
    public Guid BusinessId { get; set; } // TODO: remove?
    public Business? Business { get; set; } // TODO: remove?

    public ICollection<Channel> Channels { get; set; } = new List<Channel>();
    public List<ChannelEvent> ChannelEvents { get; set; } = new();

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? AgeLimit { get; set; }

    public string? Code { get; set; } = null!;

    public string? ExternalEventUrl { get; set; }

    /// <summary>
    /// When the event itself "opens" (i.e. you can start to enter the venue etc.)
    /// </summary>
    public DateTimeOffset OpenDate { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    //[NotMapped]
    //public Enums.EventStatus Status =>
    //    DateTimeOffset.UtcNow >= EndDate ? Enums.EventStatus.Past : Enums.EventStatus.Upcoming;

    [NotMapped]
    public Enums.EventStatus Status
    {
        get
        {
            if (StartDate <= DateTime.UtcNow && TicketsSold != MaxTicketsAvailable)
            {
                return Enums.EventStatus.OnSale;
            }
            if (!Publish)
            {
                return Enums.EventStatus.Draft;
            }
            if (StartDate <= DateTime.UtcNow && TicketsSold == MaxTicketsAvailable && MaxTicketsAvailable != null)
            {
                return Enums.EventStatus.SoldOut;
            }
            if (StartDate >= DateTime.UtcNow && TicketsSold != MaxTicketsAvailable)
            {
                return Enums.EventStatus.Scheduled;
            }
            if (StartDate <= DateTime.UtcNow && EndDate <= DateTime.UtcNow)
            {
                return Enums.EventStatus.Past;
            }
            if (Publish)
            {
                return Enums.EventStatus.Publish;
            }

            return Enums.EventStatus.Unknown;
        }
    }

    public string? Website { get; set; }

    public string? Location { get; set; }

    public string? City { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? Postcode { get; set; }

    public bool Publish { get; set; } = false;

    /// <summary>
    /// Requires TicketTypes to be included
    /// </summary>
    [NotMapped]
    public int? MaxTicketsAvailable => TicketTypes?.Sum(t => t.MaxTicketsAvailable);

    /// <summary>
    /// Requires TicketTypes to be included
    /// </summary>
    [NotMapped]
    public int? TicketsSold => TicketTypes?.Sum(t => t.TicketsSold);

    /// <summary>
    /// Requires TicketTypes to be included
    /// </summary>
    [NotMapped]
    public int? TicketsCheckedIn => TicketTypes?.Sum(t => t.TicketsCheckedIn);

    public ICollection<EventTicketType>? TicketTypes { get; set; }

    [ForeignKey(nameof(Category))]
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    [ForeignKey(nameof(Currency))]
    public string CurrencyId { get; set; } = null!;
    public Currency? Currency { get; set; }

    [ForeignKey(nameof(Image))]
    public Guid? ImageId { get; set; }
    public ImageDbModel? Image { get; set; }

    [ForeignKey(nameof(TicketBackgroundImage))]
    public Guid? TicketBackgroundImageId { get; set; }
    public ImageDbModel? TicketBackgroundImage { get; set; }

    /// <summary>
    /// Color in hex color code format
    /// </summary>
    [RegularExpression(@"^#[a-fA-F0-9]{6}$")]
    public string? TicketBackgroundColor { get; set; }

    /// <summary>
    /// Color in hex color code format
    /// </summary>
    [RegularExpression(@"^#[a-fA-F0-9]{6}$")]
    public string? TicketTextColor { get; set; }

    [ForeignKey(nameof(Creator))]
    public string? CreatorId { get; set; } = null!;

    public ApplicationUser? Creator { get; set; }

    public string? CustomEmailTemplateId { get; set; }

    // Tickets given for free

    // Daily capacity
}
