namespace Celebratix.Common.Exceptions;

public class EventEndTimePassedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "event_end_time_passed";

    public EventEndTimePassedException(string? message = null)
        : base(message)
    {
    }

    public EventEndTimePassedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
