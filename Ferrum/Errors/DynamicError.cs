using System.Diagnostics;

namespace Ferrum.Errors;

public class DynamicError : IError, IAggregateError, ITracedError
{
    private string _message;
    private IError? _innerError;
    private bool _isAggregate;
    private IEnumerable<IError> _innerErrors;
    private string? _stackTrace;
    private StackTrace? _localStackTrace;

    public DynamicError(string message, IError? innerError = null, StackTrace? localStackTrace = null)
    {
        _message = message;
        _innerError = innerError;
        _isAggregate = false;
        _innerErrors = innerError is null ? [] : [innerError];
        _stackTrace = localStackTrace?.ToString();
        _localStackTrace = localStackTrace;
    }

    public DynamicError(string message, IError? innerError = null, string? stackTrace = null)
    {
        _message = message;
        _innerError = innerError;
        _isAggregate = false;
        _innerErrors = innerError is null ? [] : [innerError];
        _stackTrace = stackTrace;
        _localStackTrace = null;
    }

    public DynamicError(string message, IEnumerable<IError>? innerErrors, string? stackTrace = null)
    {
        _message = message;
        _innerError = innerErrors?.FirstOrDefault();
        _isAggregate = false;
        _innerErrors = innerErrors;
        _stackTrace = stackTrace;
        _localStackTrace = null;
    }

    public DynamicError(string message, IEnumerable<IError>? innerErrors, StackTrace? localStackTrace = null)
    {
        _message = message;
        _innerError = innerErrors?.FirstOrDefault();
        _isAggregate = false;
        _innerErrors = innerErrors;
        _stackTrace = localStackTrace?.ToString();
        _localStackTrace = localStackTrace;
    }

    public DynamicError(string message, StackTrace localStackTrace)
        : this(message, (IError) null, localStackTrace)
    { }
    public DynamicError(string message, string stackTrace)
        : this(message, (IError) null, stackTrace)
    { }

    public string Message => _message;
    public IError? InnerError => _innerError;
    public bool IsAggregate => _isAggregate;
    public IEnumerable<IError> InnerErrors => _innerErrors;
    public string? StackTrace => _stackTrace;
    public StackTrace? LocalStackTrace => _localStackTrace;

}
