using FluentValidation.Results;

namespace Celebratix.Common.ErrorHandling
{
    public class CelebratixError<TErrorCode> : FluentResults.Error
    where TErrorCode : Enum
    {
        public CelebratixError(TErrorCode errorCode, string errorMessage) : base(errorMessage) =>
            Metadata.Add("ErrorCode", errorCode);

        public CelebratixError(ErrorCode errorCode, string errorMessage, ValidationFailure failure) : base(errorMessage)
        {
            Metadata.Add("ErrorCode", errorCode);
            Metadata.Add("AttemptedValue", failure.AttemptedValue);
            Metadata.Add("PropertyName", failure.PropertyName);
        }
    }

    public class CelebratixError : CelebratixError<ErrorCode>
    {
        public CelebratixError(ErrorCode errorCode, string errorMessage) : base(errorCode, errorMessage) { }
        public CelebratixError(ErrorCode errorCode, string errorMessage, ValidationFailure failure) : base(errorCode, errorMessage, failure) { }
    }
}
