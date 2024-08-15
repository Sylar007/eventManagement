namespace Celebratix.Common.Exceptions;

public class TicketInTransferOfferException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "ticket_in_transfer_offer";

    public TicketInTransferOfferException(string? message = null)
        : base(message)
    {
    }

    public TicketInTransferOfferException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
