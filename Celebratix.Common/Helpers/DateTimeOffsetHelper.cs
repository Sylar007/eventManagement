namespace Celebratix.Common.Helpers;

public static class DateTimeOffsetHelper
{
    public static bool IsBewteenTwoDatetimeOffsets(this DateTimeOffset dt, DateTimeOffset? start, DateTimeOffset? end)
    {
        if (start == null || end == null)
            return false;

        return dt >= start.Value && dt <= end.Value;
    }
}
