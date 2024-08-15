namespace Celebratix.Common.Extensions;

public static class StringExtensions
{
    public static string? NullIEmpty(this string value)
    {
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public static string? NullIfWhiteSpace(this string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
