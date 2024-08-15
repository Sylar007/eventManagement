using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

public class ChannelEvent : DbModelBase
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [ForeignKey(nameof(Channel))]
    public Guid ChannelId { get; set; }
#pragma warning disable CS8618
    public Channel Channel { get; set; }
#pragma warning restore CS8618
    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
#pragma warning disable CS8618
    public Event Event { get; set; }
#pragma warning restore CS8618
#pragma warning disable CS8618
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<ChannelEventTicketType> SelectedTicketTypes { get; set; }
#pragma warning restore CS8618
    public bool ResaleEnabled { get; set; } = false;
    public string? ResaleDisabledDescription { get; set; }
    public Uri? ResaleRedirectUrl { get; set; }
}