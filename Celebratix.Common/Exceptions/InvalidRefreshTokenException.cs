namespace Celebratix.Common.Exceptions;

public class InvalidRefreshTokenException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "invalid_refresh_token";

    public InvalidRefreshTokenException(string? message = null)
        : base(message)
    {
    }

    public InvalidRefreshTokenException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
