using static Celebratix.Common.Models.Enums;

namespace Celebratix.Common.Models.DTOs.Business.Events
{
    public class EventSearchRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public string? Location { get; set; }
        public EventSearchViewType SearchViewType { get; set; } = EventSearchViewType.Upcoming;
        public EventSearchStatus[]? Statuses { get; set; }
        public EventSearchSortColumn? SortColumn { get; set; }
    }
}