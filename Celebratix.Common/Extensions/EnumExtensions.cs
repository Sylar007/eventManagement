using Celebratix.Common.ErrorHandling;

namespace Celebratix.Common.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse<T>(value, out T enumValue) ? enumValue : defaultValue;
        }

        public static ErrorCode GetValidatorErrorCode(this string value) =>
            value.ToEnum(ErrorCode.GenericValidator);
    }
}
