using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.Admin.Business;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Models.DTOs.Business.Events;

namespace Celebratix.Common.Models.DTOs.Business
{
    public class BusinessTrackerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ChannelDto Channel { get; set; }
        public AffliateBusinessDto? Affiliate { get; set; }
        public BusinessDetailedBusinessDto? Business { get; set; }
        public int? Visits { get; set; }
        public int? Tickets { get; set; }
        public int? Conversion { get; set; }
        public decimal? Revenue { get; set; }
        public BusinessTrackerDto(Tracking dbModel)
        {
            Id = dbModel.Id;
            Channel = new ChannelDto(dbModel.Channel);
            Affiliate = dbModel.Affiliate != null ? new AffliateBusinessDto(dbModel.Affiliate) : null;
            Business = dbModel.Business != null ? new BusinessDetailedBusinessDto(dbModel.Business) : null;
            Name = dbModel.Name;
            Visits = dbModel.Visits;
            Tickets = dbModel.Tickets;
            Conversion = dbModel.Conversion;
            Revenue = dbModel.Revenue;
        }
    }
}
