namespace Celebratix.Common.Exceptions;

public class SlugInvalidFormatException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "slug_invalid_format";

    public SlugInvalidFormatException(string message)
        : base(message)
    {
    }

    public SlugInvalidFormatException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
