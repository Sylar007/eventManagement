namespace Celebratix.Common.Exceptions
{
    public class TicketForRefundApprovedException : Exception
    {
        /// <summary>
        /// The https://tools.ietf.org/html/rfc7807 type name of the exception
        /// </summary>
        public const string ProblemDetailType = "ticket_processed_to_refund_approved";

        public TicketForRefundApprovedException(string? message = null)
            : base(message)
        {
        }

        public TicketForRefundApprovedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}