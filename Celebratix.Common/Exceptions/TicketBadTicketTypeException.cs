namespace Celebratix.Common.Exceptions;

public class TicketBadTicketTypeException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "bad_ticket_type_on_ticket";

    public TicketBadTicketTypeException(string? message = null)
        : base(message)
    {
    }

    public TicketBadTicketTypeException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
