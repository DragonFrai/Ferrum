using Ferrum.Errors;
using Ferrum.ExceptionInterop;

namespace Ferrum;

public static class Error
{
    public static IError Failure(string message)
    {
        return new MessageError(message);
    }

    public static IError Aggregate(string message, IEnumerable<IError> errors)
    {
        return new AggregateError(message, errors);
    }

    public static IError Aggregate(string message, params IError[] errors)
    {
        return new AggregateError(message, errors);
    }

    public static IError Wrap<TError>(TError error)
    {
        return error switch
        {
            IError realError => realError,
            Exception exception => new ExceptionError(exception),
            _ => new WrappedError<TError>(error)
        };
    }

    public static IError WrapTraced<TError>(TError error)
    {
        return error switch
        {
            IError realError => realError,
            Exception exception => new ExceptionError(exception),
            _ => new WrappedTracedError<TError>(error)
        };
    }

    // public static IError OfException(Exception exception)
    // {
    //     throw new Exception("TODO");
    // }

}
