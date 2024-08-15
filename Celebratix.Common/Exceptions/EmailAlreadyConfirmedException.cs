namespace Celebratix.Common.Exceptions;

public class EmailAlreadyConfirmedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "email_already_confirmed";

    public EmailAlreadyConfirmedException(string? message = null)
        : base(message)
    {
    }

    public EmailAlreadyConfirmedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
