namespace Celebratix.Common.Exceptions;

public class PayoutsNotEnabledOnUserException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "payouts_not_enabled_on_user";

    public PayoutsNotEnabledOnUserException(string? message = null)
        : base(message)
    {
    }

    public PayoutsNotEnabledOnUserException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
