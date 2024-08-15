using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels
{
    public class Transaction : DbModelBase
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Event))]
        public int EventId { get; set; }
        public Event Event { get; set; } = default!;

        [ForeignKey(nameof(Channel))]
        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; } = default!;

        [ForeignKey(nameof(Business))]
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = default!;

        [ForeignKey(nameof(Tracking))]
        public Guid TrackingId { get; set; }
        public Tracking Tracking { get; set; } = default!;

        public DateTimeOffset TransactionDate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        public string? Status => "-";
        [NotMapped]
        public string? NoOfTickets => "-";
        [NotMapped]
        public string? OrderValue => "-";
    }
}
