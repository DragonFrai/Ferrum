namespace Ferrum;

using Errors;


public static class ErrorsErrorExtensions
{
    public static ContextError Context(this IError error, string message)
    {
        return new ContextError(message, error);
    }
}
