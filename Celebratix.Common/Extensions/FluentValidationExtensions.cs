using Celebratix.Common.ErrorHandling;
using FluentValidation.Results;

namespace Celebratix.Common.Extensions
{
    public static class FluentValidationExtensions
    {
        public static List<CelebratixError> ToErrorMessage(this List<ValidationFailure> failures)
        {
            var result = failures.Distinct().Select(error =>
                new CelebratixError(error.ErrorCode.GetValidatorErrorCode(), error.ErrorMessage, error));

            return result.Distinct().ToList();
        }
    }
}