namespace Celebratix.Common.Models.DTOs.Business.Tickets;

// The event id should be a parameter in the endpoint route instead of being passed in here
public class TicketScanningInputDto
{
    public string CipherText { get; set; } = null!;

    public DateTimeOffset Timestamp { get; set; }

    public ICollection<Guid>? AllowedTicketTypes { get; set; }
}
