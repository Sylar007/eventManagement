namespace Celebratix.Common.Exceptions;

public class UserDeletedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "user_deleted";

    public UserDeletedException(string? message = null)
        : base(message)
    {
    }

    public UserDeletedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
