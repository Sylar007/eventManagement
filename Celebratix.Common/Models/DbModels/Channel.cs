using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;
[Index(nameof(Slug), IsUnique = true)]
public class Channel : DbModelBase
{
    public Guid Id { get; set; }

    public string Slug { get; set; } = null!;

    [ForeignKey(nameof(Business))]
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Enums.ChannelTemplateTypes TemplateType { get; set; }
    public string? Color { get; set; }

    [ForeignKey(nameof(CustomBackground))]
    public Guid? CustomBackgroundId { get; set; }
    public ImageDbModel? CustomBackground { get; set; }

    public ICollection<Event> Events { get; set; } = null!;
    public List<ChannelEvent> ChannelEvents { get; set; } = new();

    public ICollection<Affiliate> AffiliateCodes { get; set; } = null!;
}
