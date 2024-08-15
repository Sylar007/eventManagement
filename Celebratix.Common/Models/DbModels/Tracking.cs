using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels
{
    public class Tracking : DbModelBase
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Channel))]
        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; } = default!;

        [ForeignKey(nameof(Affiliate))]
        public Guid AffiliateId { get; set; }
        public Affiliate Affiliate { get; set; } = default!;

        [ForeignKey(nameof(Business))]
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = default!;
        public string Name { get; set; } = string.Empty;

        [NotMapped]
        public int? Visits { get; set; }
        [NotMapped]
        public int? Tickets { get; set; }
        [NotMapped]
        public int? Conversion { get; set; }
        [NotMapped]
        public decimal? Revenue { get; set; }
    }
}
