namespace Celebratix.Common.Exceptions;

public class SlugAlreadyUsedException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "slug_already_used";

    public SlugAlreadyUsedException(string? message = null)
        : base(message)
    {
    }

    public SlugAlreadyUsedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
