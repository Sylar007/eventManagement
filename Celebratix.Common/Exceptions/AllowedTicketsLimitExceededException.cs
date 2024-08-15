namespace Celebratix.Common.Exceptions;

public class AllowedTicketsLimitExceededException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "allowed_tickets_limit_exceeded";

    public AllowedTicketsLimitExceededException(string? message = null)
        : base(message)
    {
    }

    public AllowedTicketsLimitExceededException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
