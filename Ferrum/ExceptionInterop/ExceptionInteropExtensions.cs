namespace Ferrum.ExceptionInterop;

public static class ExceptionInteropExtensions
{
    public static Exception ToException(this IError error)
    {
        return new ErrorException(error);
    }

    public static void Throw(this IError error)
    {
        throw new ErrorException(error);
    }
}
