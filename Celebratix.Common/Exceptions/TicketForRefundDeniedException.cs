namespace Celebratix.Common.Exceptions
{
    public class TicketForRefundDeniedException : Exception
    {
        /// <summary>
        /// The https://tools.ietf.org/html/rfc7807 type name of the exception
        /// </summary>
        public const string ProblemDetailType = "ticket_processed_to_refund_denied";

        public TicketForRefundDeniedException(string? message = null)
            : base(message)
        {
        }

        public TicketForRefundDeniedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}