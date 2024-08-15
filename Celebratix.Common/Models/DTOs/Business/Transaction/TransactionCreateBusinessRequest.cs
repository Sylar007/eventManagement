namespace Celebratix.Common.Models.DTOs.Business
{
    public class TransactionCreateBusinessRequest
    {
        public int EventId { get; set; }
        public Guid ChannelId { get; set; }
        public Guid TrackingId { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
