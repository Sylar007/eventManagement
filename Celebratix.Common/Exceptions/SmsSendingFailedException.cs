namespace Celebratix.Common.Exceptions;

public class SmsSendingFailedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "sms_sending_failed";

    public SmsSendingFailedException(string? message = null)
        : base(message)
    {
    }

    public SmsSendingFailedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
