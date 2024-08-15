using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.Business.Channel;

namespace Celebratix.Common.Models.DTOs.Business.Events;

public class AffliateBusinessDto
{

    /// <Required_includes>
    /// Event
    /// Orders
    /// </Required_includes>
    public AffliateBusinessDto(Affiliate dbModel)
    {
        Id = dbModel.Id;
        Code = dbModel.Code;
        Name = dbModel.Name;
        Description = dbModel.Description;
        CreatorId = dbModel.CreatorId;
        TicketsBought = dbModel.TicketsBought;
        Views = dbModel.Views;
        Revenue = new AmountDto(dbModel.BaseRevenue, dbModel.ServiceRevenue, dbModel.ApplicationRevenue);
        Channel = dbModel.Channel != null ? new ChannelBusinessDto(dbModel.Channel) : null;
        BusinessId = dbModel.Channel?.BusinessId ?? Guid.Empty;
    }

    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int EventId { get; set; }

    public string? EventName { get; set; }

    public string? Description { get; set; }

    public string Code { get; set; }

    public string? CreatorId { get; set; }

    /// <summary>
    /// Number of tickets bought with the affiliate code
    /// </summary>
    public int? TicketsBought { get; set; }

    public int Views { get; set; }

    public AmountDto Revenue { get; set; }

    public ChannelBusinessDto? Channel { get; set; }

    public Guid BusinessId { get; set; }
}
