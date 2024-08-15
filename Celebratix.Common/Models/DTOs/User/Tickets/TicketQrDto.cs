using Celebratix.Common.Models.DbModels;
namespace Celebratix.Common.Models.DTOs.User.Tickets;
using MarketplaceAction = UserAction<Guid?, MarketplaceUnavailableReason>;
using TransferAction = UserAction<Guid?, TransferUnavailableReason>;
using QrAction = UserAction<string, QrUnavailableReason>;
using CollectableAction = UserAction<dynamic, CollectableUnavailableReason>;

public enum MarketplaceUnavailableReason
{
    CheckedIn = 1,
    SoldOrTransferred,
    EventPassed,
    TicketTransferOffer,
}

public enum TransferUnavailableReason
{
    CheckedIn = 1,
    SoldOrTransferred,
    EventPassed,
    MarketplaceListing,
}

public enum QrUnavailableReason
{
    CheckedIn = 1,
    SoldOrTransferred,
    EventPassed,
    MarketplaceListing,
}

public enum CollectableUnavailableReason
{
    EventOngoing = 1,
}

public class TicketQrActions
{
    public TicketQrActions(Ticket dbModel, string ownerId, string key)
    {
        var soldOrTransferred = dbModel.OwnerId != ownerId;
        var eventPassed = dbModel.TicketType!.Event!.EndDate < DateTime.UtcNow;
        if (dbModel.CheckedIn)
            Marketplace = new MarketplaceAction(MarketplaceUnavailableReason.CheckedIn);
        else if (dbModel.ActiveTicketTransferOfferId != null)
            Marketplace = new MarketplaceAction(MarketplaceUnavailableReason.TicketTransferOffer);
        else if (soldOrTransferred)
            Marketplace = new MarketplaceAction(MarketplaceUnavailableReason.SoldOrTransferred);
        else if (eventPassed)
            Marketplace = new MarketplaceAction(MarketplaceUnavailableReason.EventPassed);
        else
            Marketplace = new MarketplaceAction(dbModel.ActiveMarketplaceListingId);

        if (dbModel.CheckedIn)
            Transfer = new TransferAction(TransferUnavailableReason.CheckedIn);
        else if (dbModel.ActiveMarketplaceListingId != null)
            Transfer = new TransferAction(TransferUnavailableReason.MarketplaceListing);
        else if (soldOrTransferred)
            Transfer = new TransferAction(TransferUnavailableReason.SoldOrTransferred);
        else if (eventPassed)
            Transfer = new TransferAction(TransferUnavailableReason.EventPassed);
        else
            Transfer = new TransferAction(dbModel.ActiveTicketTransferOfferId);

        if (dbModel.CheckedIn)
            Qr = new QrAction(QrUnavailableReason.CheckedIn);
        else if (dbModel.ActiveMarketplaceListingId != null)
            Qr = new QrAction(QrUnavailableReason.MarketplaceListing);
        else if (soldOrTransferred)
            Qr = new QrAction(QrUnavailableReason.SoldOrTransferred);
        else if (eventPassed)
            Qr = new QrAction(QrUnavailableReason.EventPassed);
        else
            Qr = new QrAction(key);

        if (!eventPassed)
            Collectible = new CollectableAction(CollectableUnavailableReason.EventOngoing);
        else
            Collectible = new CollectableAction(null);
    }
    /// <summary>
    /// The .data field contains the string that represents the QR code
    /// </summary>
    public QrAction Qr { get; }
    /// <summary>
    /// The .data field contains the GUID of the marketplace listing if any
    /// </summary>
    public MarketplaceAction Marketplace { get; }
    /// <summary>
    /// The .data field contains the GUID of the ticket transfer offer if any
    /// </summary>
    public TransferAction Transfer { get; }
    /// <summary>
    /// The .data field is always null
    /// </summary>
    public CollectableAction Collectible { get; }
}

/// <summary>
/// Required includes:
/// TicketType
/// TicketType.Event
/// </summary>
public class TicketQrDto : TicketQrActions
{
    public Guid Id { get; set; }

    [Obsolete($"Use the {nameof(Qr)} field instead")]
    public bool SoldOrTransferred { get; set; }
    public string TicketTypeName { get; set; }
    /// <summary>
    /// Is only provided if the ticket isn't already checked in
    /// </summary>
    [Obsolete($"Use the {nameof(Qr)} field instead")]
    public string? Key { get; set; } = null!;
    public bool? CheckedIn { get; set; }

    [Obsolete($"Use the {nameof(Marketplace)} field instead")]
    public Guid? ActiveMarketplaceListingId { get; set; }
    [Obsolete($"Use the {nameof(Marketplace)} field instead")]
    public bool? IsInMarketplaceListing { get; set; }

    [Obsolete($"Use the {nameof(Transfer)} field instead")]
    public Guid? ActiveTicketTransferOfferId { get; set; }
    [Obsolete($"Use the {nameof(Transfer)} field instead")]
    public bool? IsInTicketTransferOffer { get; set; }

    public TicketQrDto(Ticket dbModel, string ownerId, string key) : base(dbModel, ownerId, key)
    {
        Id = dbModel.Id;
#pragma warning disable 0618 // disable use of obsolete variable warning
        SoldOrTransferred = dbModel.OwnerId != ownerId;
        TicketTypeName = dbModel.TicketType!.Name;

        if (!SoldOrTransferred)
        {
            Key = Qr.Data;
            CheckedIn = dbModel.CheckedIn;
            ActiveMarketplaceListingId = dbModel.ActiveMarketplaceListingId;
            IsInMarketplaceListing = dbModel.ActiveMarketplaceListingId != null;
            ActiveTicketTransferOfferId = dbModel.ActiveTicketTransferOfferId;
            IsInTicketTransferOffer = dbModel.ActiveTicketTransferOfferId != null;
        }
#pragma warning restore 0618
    }
}
