namespace Celebratix.Common.Exceptions;

public class RequestedTicketsNotAvailableException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "requested_tickets_not_available";

    public RequestedTicketsNotAvailableException(string? message = null)
        : base(message)
    {
    }

    public RequestedTicketsNotAvailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
