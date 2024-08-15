using Celebratix.Common.ErrorHandling;

namespace Celebratix.Models;

public class OperationError
{
    public ErrorCode Code { get; }
    public string Message { get; }
    public ValidationError? ValidationError { get; } = default;

    public OperationError(ErrorCode code, string message)
    {
        Code = code;
        Message = message;
    }

    public OperationError(ErrorCode code, string message, ValidationError? validationError)
    {
        Code = code;
        Message = message;
        ValidationError = validationError;
    }
}

public record ValidationError(string PropertyName, object AttemptedValue);