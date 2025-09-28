namespace Ferrum;

using Errors;


public static class ErrorsErrorExtensions
{
    public static ContextError Context(this IError error, string message)
    {
        return new ContextError(message, error);
    }

    /// <summary>
    /// Adds context to the error.
    ///
    /// If the source error chain does not contain a stack trace, adds a stack trace;
    /// if there is at least one stack trace, does not add a stack trace.
    /// </summary>
    /// <param name="error"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ContextError ContextOnce(this IError error, string message)
    {
        return error.IsChainHasStakTrace() ? new ContextError(message, error) : new TracedContextError(message, error);
    }

    public static TracedContextError ContextTraced(this IError error, string message)
    {
        return new TracedContextError(message, error);
    }
}
