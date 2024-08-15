using Celebratix.Common.Models.DTOs.Admin.Business;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Models.DTOs.Business.Events;

namespace Celebratix.Common.Models.DTOs.Business.Transaction
{
    public class TransactionBusinessDto
    {
        public Guid Id { get; set; }
        public EventBasicBusinessDto? Event { get; set; }
        public ChannelDto? Channel { get; set; }
        public BusinessDetailedBusinessDto? Business { get; set; }
        public BusinessTrackerDto? Tracking { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Status { get; set; }
        public string? NoOfTickets { get; set; }
        public string? OrderValue { get; set; }
        public TransactionBusinessDto(DbModels.Transaction dbModel)
        {
            Id = dbModel.Id;
            Event = dbModel.Event != null ? new EventBasicBusinessDto(dbModel.Event) : null;
            Channel = dbModel.Channel != null ? new ChannelDto(dbModel.Channel) : null;
            Tracking = dbModel.Tracking != null ? new BusinessTrackerDto(dbModel.Tracking) : null;
            Business = dbModel.Business != null ? new BusinessDetailedBusinessDto(dbModel.Business) : null;
            FullName = dbModel.FullName;
            Email = dbModel.Email;
            TransactionDate = dbModel.TransactionDate;
            Status = dbModel.Status;
            NoOfTickets = dbModel.NoOfTickets;
            OrderValue = dbModel.OrderValue;
        }
    }
}
