using System.Diagnostics;

namespace Ferrum.ExceptionInterop;


// TODO: Handle AggregateException converting
public class ExceptionError : ITracedError
{
    private readonly Exception _exception;
    private readonly ExceptionError? _innerError;

    public ExceptionError(Exception exception)
    {
        _exception = exception;
        var innerException = exception.InnerException;
        _innerError = innerException switch
        {
            null => null,
            _ => new ExceptionError(innerException)
        };
    }

    public string Message => _exception.Message;
    public IError? InnerError => _innerError;
    public string? StackTrace => _exception.StackTrace;
    public bool HasStackTrace => _exception.StackTrace is not null;
    public StackTrace? LocalStackTrace => null;
}
