using Celebratix.Common.Models.DbModels;

namespace Celebratix.Common.Models.DTOs.Business.Tickets;

public class TicketScanningResponseDto
{
    /// <summary>
    /// Required includes:
    /// TicketType
    /// </summary>
    public TicketScanningResponseDto(Ticket dbModel, string ownerName)
    {
        TicketTypeName = dbModel.TicketType!.Name;
        OwnerName = ownerName;
    }

    public string TicketTypeName { get; set; }

    public string OwnerName { get; set; }
}
