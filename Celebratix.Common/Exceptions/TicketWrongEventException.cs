namespace Celebratix.Common.Exceptions;

public class TicketWrongEventException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "bad_event_on_ticket";

    public TicketWrongEventException(string? message = null)
        : base(message)
    {
    }

    public TicketWrongEventException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
