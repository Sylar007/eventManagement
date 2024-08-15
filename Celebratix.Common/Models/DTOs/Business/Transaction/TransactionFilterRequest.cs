namespace Celebratix.Common.Models.DTOs.Business
{
    public class TransactionFilterRequest
    {
        public DateTimeOffset? TransactionDateFrom { get; set; }
        public DateTimeOffset? TransactionDateTo { get; set; }
        public string[]? Statuses { get; set; }
        public int? EventId { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? TrackingId { get; set; }
    }
}