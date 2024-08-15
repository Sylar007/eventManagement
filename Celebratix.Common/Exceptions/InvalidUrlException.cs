namespace Celebratix.Common.Exceptions;

public class InvalidUrlException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "invalid_url";

    public InvalidUrlException(string? message = null)
        : base(message)
    {
    }

    public InvalidUrlException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
