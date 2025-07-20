using System.Collections.ObjectModel;
using Ferrum.Errors;

namespace Ferrum.ExceptionInterop;


public interface IExceptionError : ITracedError
{
    public Exception Exception { get; }
}


internal static class ExceptionErrorImpl
{
    public static IExceptionError Create(Exception ex)
    {
        return ex switch
        {
            AggregateException aggregateException => new AggregateExceptionError(aggregateException),
            _ => new ExceptionError(ex)
        };
    }
}


// TODO: Handle AggregateException converting
internal class ExceptionError : BaseError, IExceptionError
{
    private readonly Exception _exception;
    private readonly IExceptionError? _innerError;

    private static IExceptionError? ExceptionToErrorNullable(Exception? innerException)
    {
        return innerException is null ? null : ExceptionErrorImpl.Create(innerException);
    }

    public ExceptionError(Exception exception)
    {
        _exception = exception;
        _innerError = ExceptionToErrorNullable(exception.InnerException);
    }

    public override string Message => _exception.Message;
    public override IError? InnerError => _innerError;

    public string? StackTrace => _exception.StackTrace;
    public bool HasStackTrace => _exception.StackTrace is not null;

    public Exception Exception => _exception;
}

internal class AggregateExceptionError : BaseError, IAggregateError, IExceptionError
{
    private readonly AggregateException _aggregateException;
    private readonly IExceptionError? _innerError;
    private readonly IExceptionError[] _innerErrors;
    private IReadOnlyCollection<IExceptionError>? _innerErrorsRocView;

    private static IExceptionError[] HandleInnerExceptions(IReadOnlyCollection<Exception> innerExceptions)
    {
        ArgumentNullException.ThrowIfNull(innerExceptions);
        var errors = new IExceptionError[innerExceptions.Count];
        var index = 0;
        foreach (var innerException in innerExceptions)
        {
            errors[index] = ExceptionErrorImpl.Create(innerException);
            ++index;
        }
        return errors;
    }

    public AggregateExceptionError(AggregateException aggregateException)
    {
        _aggregateException = aggregateException;
        var innerErrors = HandleInnerExceptions(aggregateException.InnerExceptions);
        _innerErrors = innerErrors;
        _innerError = innerErrors.Length != 0 ? innerErrors[0] : null;
        _innerErrorsRocView = null;
    }

    public override string Message => _aggregateException.Message;
    public override IError? InnerError => _innerError;

    public string? StackTrace => _aggregateException.StackTrace;
    public bool HasStackTrace => _aggregateException.StackTrace is not null;

    public Exception Exception => _aggregateException;

    public IReadOnlyCollection<IError>? InnerErrors
    {
        get
        {
            if (_innerErrorsRocView is not null)
            {
                return _innerErrorsRocView;
            }
            var rocView = new ReadOnlyCollection<IExceptionError>(_innerErrors);
            _innerErrorsRocView = rocView;
            return rocView;
        }
    }
}

