namespace Celebratix.Common.Exceptions;

public class TransferOfferNotAvailableException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "transfer_offer_not_available";

    public TransferOfferNotAvailableException(string? message = null)
        : base(message)
    {
    }

    public TransferOfferNotAvailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
