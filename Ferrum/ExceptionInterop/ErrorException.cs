namespace Ferrum.ExceptionInterop;



public interface IErrorException
{
    public IError Error { get; }
}


internal static class ErrorExceptionImpl
{
    internal static Exception Create(IError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        return error switch
        {
            IAggregateError { InnerErrors: not null } aggregateError => new AggregateErrorException(aggregateError),
            _ => new ErrorException(error)
        };
    }

}

internal class ErrorException : Exception, IErrorException
{
    private readonly IError _error;

    public IError Error => _error;

    private static Exception? InnerErrorCtorArg(IError? innerError)
    {
        return innerError is null ? null : ErrorExceptionImpl.Create(innerError);
    }

    internal ErrorException(IError error)
        : base(
            error.Message,
            InnerErrorCtorArg(error.InnerError)
        )
    {
        _error = error;
    }

    // TODO?: Override StackTrace to error stack trace ???
}

internal class AggregateErrorException : AggregateException, IErrorException
{
    private readonly IAggregateError _aggregateError;

    public IAggregateError AggregateError => _aggregateError;

    public IError Error => _aggregateError;

    private static Exception[] InnerErrorsCtorArg(IReadOnlyCollection<IError>? innerErrors)
    {
        if (innerErrors is null)
        {
            throw new ArgumentException("InnerErrors is null (IAggregateError is not aggregated)");
        }

        var exceptions = new Exception[innerErrors.Count];
        var index = 0;
        foreach (var error in innerErrors)
        {
            exceptions[index] = ErrorExceptionImpl.Create(error);
            ++index;
        }
        return exceptions;
    }

    internal AggregateErrorException(IAggregateError error)
        : base(
            error.Message,
            InnerErrorsCtorArg(error.InnerErrors)
        )
    {
        _aggregateError = error;
    }

    // TODO?: Override StackTrace to error stack trace ???
}
