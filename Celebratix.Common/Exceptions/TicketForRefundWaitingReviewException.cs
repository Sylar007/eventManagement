namespace Celebratix.Common.Exceptions
{
    public class TicketForRefundWaitingReviewException : Exception
    {
        /// <summary>
        /// The https://tools.ietf.org/html/rfc7807 type name of the exception
        /// </summary>
        public const string ProblemDetailType = "ticket_processed_to_refund_waitingreview";

        public TicketForRefundWaitingReviewException(string? message = null)
            : base(message)
        {
        }

        public TicketForRefundWaitingReviewException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}