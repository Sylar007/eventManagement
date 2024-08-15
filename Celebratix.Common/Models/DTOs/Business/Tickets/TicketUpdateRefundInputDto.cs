namespace Celebratix.Common.Models.DTOs.Business.Tickets;
public class TicketUpdateRefundInputDto
{
    public Guid Id { get; set; }
    public Enums.RefundType Status { get; set; }
}