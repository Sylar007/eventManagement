using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class EventCreateBusinessDto
{
    /// <summary>
    /// Can be used instead of the id to fetch the event
    /// </summary>
    public string? CustomSlug { get; set; }

    public bool Visible { get; set; } = false;

    /// <summary>
    /// A comma separated l of channel ids
    /// Retool doesn't support forms with > 1 entry of the same key https://datatracker.ietf.org/doc/html/rfc7578#section-5.2
    /// So we allow a comma separated list of channel ids
    /// </summary>
    public string ChannelIdsCsv { get; set; } = "";

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Only positive numbers allowed")]
    public int? AgeLimit { get; set; }

    public string? ExternalEventUrl { get; set; }

    public DateTimeOffset OpenDate { get; set; }

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public string? Website { get; set; }

    public string? Location { get; set; }

    public string? City { get; set; }

    public int? CategoryId { get; set; }

    public string CurrencyId { get; set; } = null!;

    public IFormFile? Image { get; set; }

    public IFormFile? TicketBackgroundImage { get; set; }

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
}
