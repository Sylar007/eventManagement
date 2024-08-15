namespace Celebratix.Common.Exceptions;

public class ListingNotAvailableException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "listing_not_available";

    public ListingNotAvailableException(string? message = null)
        : base(message)
    {
    }

    public ListingNotAvailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
