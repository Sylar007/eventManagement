namespace Celebratix.Common.Exceptions;

public class BadOrderStatusException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "bad_order_status";

    public BadOrderStatusException(string? message = null)
        : base(message)
    {
    }

    public BadOrderStatusException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
