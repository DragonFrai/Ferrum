using System.Diagnostics;

namespace Ferrum;


public static class ErrorExtensions
{
    public static IEnumerable<IError> Chain(this IError error)
    {
        var current = error;
        while (current is not null)
        {
            yield return current;
            current = current.InnerError;
        }
    }

    public static IError GetRoot(this IError error)
    {
        var current = error;
        var inner = current.InnerError;
        while (inner is not null)
        {
            current = inner;
            inner = inner.InnerError;
        }
        return current;
    }

    public static string? GetStackTrace(this IError error)
    {
        return (error as ITracedError)?.StackTrace;
    }

    public static bool GetIsAggregate(this IError error)
    {
        return error is IAggregateError { InnerErrors: not null };
    }

    public static IReadOnlyCollection<IError> GetInnerErrors(this IError error)
    {
        if (error is IAggregateError { InnerErrors: not null } aggregateError)
        {
            return aggregateError.InnerErrors;
        }

        var innerError = error.InnerError;
        if (innerError is not null)
        {
            return [innerError];
        }

        return [];
    }

}
