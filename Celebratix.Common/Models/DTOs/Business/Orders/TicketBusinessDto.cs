using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Orders
{
    public class TicketBusinessDto
    {
        public TicketBusinessDto(Ticket dbModel)
        {
            Id = dbModel.Id;
            OwnerName = dbModel.Owner?.FullName!;
            CheckedIn = dbModel.CheckedIn;
        }

        public Guid Id { get; set; }
        public string OwnerName { get; set; }
        public bool CheckedIn { get; set; }
    }
}
