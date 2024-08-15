using static Celebratix.Common.Models.Enums;

namespace Celebratix.Common.Models.DTOs.Business
{
    public class BusinessTrackerSearchRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public Guid? Channel { get; set; }
        public Guid[]? Affiliates { get; set; }
        public TrackingSearchSortColumn? SortColumn { get; set; }
    }
}
