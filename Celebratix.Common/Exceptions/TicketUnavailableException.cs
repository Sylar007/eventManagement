namespace Celebratix.Common.Exceptions;

public class TicketUnavailableException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "ticket_is_unavailable_to_refund";

    public TicketUnavailableException(string? message = null)
        : base(message)
    {
    }

    public TicketUnavailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}