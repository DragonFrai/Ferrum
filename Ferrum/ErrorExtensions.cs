using Ferrum.ExceptionInterop;

namespace Ferrum;

using Ferrum.Errors;


public static class ErrorExtensions
{
    public static IError Context(this IError error, string message)
    {
        return new ContextError(message, error);
    }

    public static Exception ToException(this IError error)
    {
        return new ErrorException(error);
    }

    public static void Throw(this IError error)
    {
        throw new ErrorException(error);
    }


}
