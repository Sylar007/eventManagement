namespace Celebratix.Common.ErrorHandling;
public class ErrEnum<T>
where T : Enum
{
    public T Code { get; }
    public string? Message { get; }

    public ErrEnum(T enumValue, string? message)
    {
        Code = enumValue;
        Message = message;
    }

    public static implicit operator ErrEnum<T>(T enumValue)
    {
        return new ErrEnum<T>(enumValue, null);
    }
}
public static class ErrEnum
{
    public static ErrEnum<T> New<T>(T v, string? message = null) where T : Enum
    {
        return new ErrEnum<T>(v, message);
    }
}
