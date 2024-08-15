using System.ComponentModel.DataAnnotations.Schema;

namespace Celebratix.Common.Models.DbModels;

/// <summary>
/// An offer can also be accepted, and is then a "Transferred ticket"
/// </summary>
public class TicketTransferOffer : DbModelBase
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Ticket))]
    public Guid TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    [InverseProperty(nameof(Order.TicketTransferOffer))]
    public Order? FulfilledByOrder { get; set; }

    /// <summary>
    /// Nullable as user can potentially be deleted
    /// </summary>
    [ForeignKey(nameof(Transferor))]
    public string? TransferorId { get; set; }
    public ApplicationUser? Transferor { get; set; }

    [ForeignKey(nameof(Receiver))]
    public string? ReceiverId { get; set; }
    public ApplicationUser? Receiver { get; set; }

    public DateTimeOffset? TransferredAt { get; set; }

    [NotMapped]
    public bool Transferred => TransferredAt != null;

    [NotMapped]
    public bool Available => !Transferred && !Cancelled;

    public string Code { get; set; } = null!;

    public bool Cancelled { get; set; } = false;
}
