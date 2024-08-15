namespace Celebratix.Common.Exceptions;

public class JwtTokenInvalidException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "jwt_token_invalid";

    public JwtTokenInvalidException(string? message = null)
        : base(message)
    {
    }

    public JwtTokenInvalidException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
