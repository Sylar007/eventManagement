namespace Celebratix.Common.Extensions;

public static class ListExtensions
{
    public static string ToCommaSeparatedString<T>(this IEnumerable<T> list)
    {
        return string.Join(", ", list);
    }
}
