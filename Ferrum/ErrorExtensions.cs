using Ferrum.ExceptionInterop;

namespace Ferrum;

using Ferrum.Errors;


public static class ErrorExtensions
{
    public static IError Context(this IError error, string message)
    {
        return new ContextError(message, error);
    }
}
