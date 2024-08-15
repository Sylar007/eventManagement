namespace Celebratix.Common.Exceptions;

public class ObjectAlreadyExistsException : Exception
{
    /// <summary>
    /// The https://tools.ietf.org/html/rfc7807 type name of the exception
    /// </summary>
    public const string ProblemDetailType = "object_already_exists";

    public ObjectAlreadyExistsException(string message)
        : base(message)
    {
    }

    public ObjectAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
