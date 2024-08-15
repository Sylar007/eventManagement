using Celebratix.Common.Models.DTOs.User.Tickets;

namespace Celebratix.Common.Models;

public static class Enums
{
    public enum Role
    {
        SuperAdmin = 1, // Super admin, has access to everything
        Business = 2,
        TicketChecker,
    }

    // Added as helpers for controller authorization
    public const string SuperAdmin = nameof(Role.SuperAdmin);
    public const string Business = nameof(Role.Business);
    public const string TicketChecker = nameof(Role.TicketChecker);

    /// <summary>
    /// Measured in number of months
    /// </summary>
    public enum TimeInterval
    {
        Monthly = 1,
        BiMonthly = 2,
        HalfYearly = 6,
        Yearly = 12
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum OrderStatus
    {
        AwaitingPaymentInfo = 0,
        RequiresUserAction = 6,
        Processing = 1,
        Completed = 2,
        // Currently unused status for stripe implementation. Failed intents leads to order going back to awaiting payment
        Failed = 3,
        Cancelled = 4,
        Other = 5
    }

    public enum OrderType
    {
        PrimaryMarket = 1,
        Marketplace = 2,
        Transfer = 3
    }

    public enum EventStatus
    {
        Draft = 0,
        Publish = 1,
        OnSale = 2,
        SoldOut = 3,
        Scheduled = 4,
        Past = 5,
        Upcoming = 6,
        Unknown = 7
    }
    public enum SortDirection
    {
        Ascending,
        Descending
    }
    //public enum TicketTypeStatus
    //{
    //    Automatic = 1,
    //    SoldOut = 2
    //}

    public enum EventSearchViewType
    {
        Upcoming,
        Past
    }

    public enum EventSearchStatus
    {
        Draft = 0,
        Publish = 1,
        OnSale = 2,
        SoldOut = 3,
        Scheduled = 4
    }

    public enum EventSearchSortColumn
    {
        EventNameAsc,
        EventNameDesc,
        StartDateAsc,
        StartDateDesc,
        EndDateAsc,
        EndDateDesc,
        CreatedAtAsc,
        CreatedAtDesc
    }

    public enum TrackingSearchSortColumn
    {
        NameAsc,
        NameDesc,
        CreatedAtAsc,
        CreatedAtDesc
    }

    public enum TransactionSearchSortColumn
    {
        TransactiondateAsc,
        TransactiondateDesc,
        CreatedAtAsc,
        CreatedAtDesc
    }

    public enum ChannelTemplateTypes
    {
        Widget
    }

    public enum ScanFailReason
    {
        CheckedIn = 1,
        SoldOrTransferred,
        EventPassed,
        MarketplaceListing,

        QrTimeExpired,
        QrCorrupted,
        ForbiddenTicketType,
        ForbiddenEvent,
        UserNotFound,
        TicketNotFound,
    };

    public enum RefundType
    {
        Default = 0,
        WaitingForReview = 1,
        Denied = 2,
        Approved = 3
    }
}
