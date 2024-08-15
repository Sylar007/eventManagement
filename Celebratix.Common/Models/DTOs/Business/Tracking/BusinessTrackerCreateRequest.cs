namespace Celebratix.Common.Models.DTOs.Business
{
    public class BusinessTrackerCreateRequest
    {
        public Guid ChannelId { get; set; }
        public Guid AffiliateId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
