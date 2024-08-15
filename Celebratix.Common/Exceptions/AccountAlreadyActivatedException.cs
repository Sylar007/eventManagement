namespace Celebratix.Common.Exceptions;

public class AccountAlreadyActivatedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "account_already_activated";

    public AccountAlreadyActivatedException(string? message = null)
        : base(message)
    {
    }

    public AccountAlreadyActivatedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
