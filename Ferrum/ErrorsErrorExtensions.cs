namespace Ferrum;

using Errors;


public static class ErrorsErrorExtensions
{
    public static ContextError Context(this IError error, string message)
    {
        return new ContextError(message, error);
    }

    public static TracedContextError ContextTraced(this IError error, string message)
    {
        return new TracedContextError(message, error);
    }
}
