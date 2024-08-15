namespace Celebratix.Common.Exceptions;

public class TicketSoldOrTransferredException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "ticket_sold_or_transferred";

    public TicketSoldOrTransferredException(string? message = null)
        : base(message)
    {
    }

    public TicketSoldOrTransferredException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
