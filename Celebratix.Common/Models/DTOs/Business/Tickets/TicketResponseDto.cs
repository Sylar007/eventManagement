using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Tickets
{
    public class TicketResponseDto
    {
        public TicketResponseDto(Ticket dbModel)
        {
            Id = dbModel.Id;
            Owner = dbModel.Owner;
            TicketType = dbModel.TicketType;
            CheckedIn = dbModel.CheckedIn;
            Refund = dbModel.Refund;
        }

        public Guid Id { get; set; }
        public EventTicketType? TicketType { get; set; }
        public ApplicationUser? Owner { get; set; }
        public bool CheckedIn { get; set; }
        public Enums.RefundType Refund { get; set; }
    }
}