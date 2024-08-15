
using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Orders
{
    public class OrderDetailedBusinessDto : OrderBasicBusinessDto
    {
        public OrderDetailedBusinessDto(Order dbModel) : base(dbModel)
        {
            Tickets = dbModel.Tickets?.Select(t => new TicketBusinessDto(t)).ToList();
        }

        public ICollection<TicketBusinessDto>? Tickets { get; set; }
    }
}
