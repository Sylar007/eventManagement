using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Celebratix.Common.Models.DbModels
{
    public class ChannelEventTicketType : DbModelBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey(nameof(ChannelEvent))]
        public Guid ChannelEventId { get; set; }
        public ChannelEvent ChannelEvent { get; set; }
        [ForeignKey(nameof(EventTicketType))]
        public Guid EventTicketTypeId { get; set; }
        public EventTicketType EventTicketType { get; set; }
    }
}
