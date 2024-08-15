namespace Celebratix.Common.Exceptions;

public class PhoneNotConfirmedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "phone_not_confirmed";

    public PhoneNotConfirmedException(string? message = null)
        : base(message)
    {
    }

    public PhoneNotConfirmedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
