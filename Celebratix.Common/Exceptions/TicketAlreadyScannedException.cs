﻿namespace Celebratix.Common.Exceptions;

public class TicketAlreadyScannedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "ticket_already_scanned";

    public TicketAlreadyScannedException(string? message = null)
        : base(message)
    {
    }

    public TicketAlreadyScannedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
