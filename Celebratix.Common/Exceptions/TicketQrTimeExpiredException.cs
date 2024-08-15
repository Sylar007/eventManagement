namespace Celebratix.Common.Exceptions;

public class TicketQrTimeExpiredException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "ticket_qr_time_expired";

    public TicketQrTimeExpiredException(string? message = null)
        : base(message)
    {
    }

    public TicketQrTimeExpiredException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
