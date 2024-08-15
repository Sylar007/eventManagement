using static Celebratix.Common.Models.Enums;

namespace Celebratix.Common.Models.DTOs.Business.Tickets
{
    public class TicketSearchRefundInputDto
    {
        public RefundType[]? Statuses { get; set; }
        public int[]? EventIds { get; set; }
    }
}