using static Celebratix.Common.Models.Enums;

namespace Celebratix.Common.Models.DTOs.Business
{
    public class TransactionSearchRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public DateTimeOffset? TransactionDateFrom { get; set; }
        public DateTimeOffset? TransactionDateTo { get; set; }
        public string[]? Statuses { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? TrackingId { get; set; }
        public TransactionSearchSortColumn? SortColumn { get; set; }
    }
}
