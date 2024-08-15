namespace Celebratix.Common.Exceptions;

public class NotMemberOfBusinessException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "not_member_of_business";

    public NotMemberOfBusinessException(string? message = null)
        : base(message)
    {
    }

    public NotMemberOfBusinessException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
