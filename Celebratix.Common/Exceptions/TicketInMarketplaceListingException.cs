namespace Celebratix.Common.Exceptions;

public class TicketInMarketplaceListingException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "ticket_in_marketplace_listing";

    public TicketInMarketplaceListingException(string? message = null)
        : base(message)
    {
    }

    public TicketInMarketplaceListingException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
