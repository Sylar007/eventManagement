using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Extensions;
using FluentResults;
using F = CSharpFunctionalExtensions;

namespace Celebratix.Models;

public class OperationResult<T>
{
    public OperationResult()
    {

    }

    public OperationResult(Result<T> result)
    {
        if (result.Errors.Any())
        {
            Errors = result.Errors
                .Select(error =>
                {
                    var errorCode = error.Metadata.ContainsKey("ErrorCode")
                        ? Convert.ToString(error.Metadata["ErrorCode"])!.ToEnum(ErrorCode.Generic)
                        : ErrorCode.Generic;

                    var propertyName = error.Metadata.ContainsKey("PropertyName")
                        ? Convert.ToString(error.Metadata["PropertyName"])
                        : null;

                    var attemptedValue = error.Metadata.ContainsKey("AttemptedValue")
                        ? error.Metadata["AttemptedValue"]
                        : null;

                    var validationError = propertyName != null || attemptedValue != null
                        ? new ValidationError(propertyName!, attemptedValue!)
                        : null;

                    return new OperationError(errorCode, error.Message, validationError);
                })
                .ToList();
        }
        else
        {
            Data = result.Value;
        }
    }

    public bool IsSuccess => !Errors.Any();
    public List<OperationError> Errors { get; init; } = new();
    public T? Data { get; init; }
}

public class ResultDto<TValue, TError>
{
    public ResultDto(TValue value)
    {
        Value = value;
    }
    public ResultDto(TError error)
    {
        Error = error;
    }

    public ResultDto(F.Result<TValue, TError> result)
    {
        if (result.IsSuccess)
            Value = result.Value;
        else
            Error = result.Error;
    }
    [MemberNotNullWhen(false, "Error")]
    [MemberNotNullWhen(true, "Value")]
    public bool Ok => Error == null;
    public TError? Error { get; }
    public TValue? Value { get; }
}
