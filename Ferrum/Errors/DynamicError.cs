using System.Collections.ObjectModel;
using System.Diagnostics;
// ReSharper disable ConvertToAutoProperty

namespace Ferrum.Errors;


public class DynamicError : BaseError, IAggregateError, ITracedError
{
    private readonly string _message;
    private readonly IError? _innerError;
    private readonly IError[]? _innerErrors;
    private StackTraceCell _stackTraceCell;

    private IReadOnlyCollection<IError>? _innerErrorsRocView;

    internal DynamicError(string? message, object? inners, object? stacks)
    {
        _message = Helpers.MessageOrDefault<DynamicError>(message);
        switch (inners)
        {
            case null:
                _innerError = null;
                _innerErrors = null;
                break;
            case IError innerError:
                _innerError = innerError;
                _innerErrors = null;
                break;
            case IError[] innerErrors:
                _innerError = innerErrors.FirstOrDefault();
                _innerErrors = innerErrors;
                break;
            default:
                throw new InvalidOperationException();
        }

        _stackTraceCell = stacks switch
        {
            null => new StackTraceCell(null, null),
            string stackTrace => new StackTraceCell(null, stackTrace),
            StackTrace localStackTrace => new StackTraceCell(localStackTrace, null),
            _ => throw new InvalidOperationException()
        };
    }

    // ReSharper disable RedundantCast

    public DynamicError(string message, IError innerError, StackTrace localStackTrace)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) Helpers.PassNotNull(innerError, nameof(innerError)),
            (object?) Helpers.PassNotNull(localStackTrace, nameof(localStackTrace))
        )
    { }

    public DynamicError(string message, IError innerError, string stackTrace)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) Helpers.PassNotNull(innerError, nameof(innerError)),
            (object?) Helpers.PassNotNull(stackTrace, nameof(stackTrace))
        )
    { }

    public DynamicError(string message, IError[] innerErrors, StackTrace localStackTrace)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) Helpers.PassNotNull(innerErrors, nameof(innerErrors)),
            (object?) Helpers.PassNotNull(localStackTrace, nameof(localStackTrace))
        )
    { }

    public DynamicError(string message, IError[] innerErrors, string stackTrace)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) Helpers.PassNotNull(innerErrors, nameof(innerErrors)),
            (object?) Helpers.PassNotNull(stackTrace, nameof(stackTrace))
        )
    { }

    public DynamicError(string message, StackTrace localStackTrace)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) null,
            (object?) Helpers.PassNotNull(localStackTrace, nameof(localStackTrace))
        )
    { }

    public DynamicError(string message, string stackTrace)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) null,
            (object?) Helpers.PassNotNull(stackTrace, nameof(stackTrace))
        )
    { }

    public DynamicError(string message, IError innerError)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) Helpers.PassNotNull(innerError, nameof(innerError)),
            (object?) null
        )
    { }

    public DynamicError(string message, IError[] innerErrors)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) Helpers.PassNotNull(innerErrors, nameof(innerErrors)),
            (object?) null
        )
    { }

    public DynamicError(string message)
        : this
        (
            (string?) Helpers.PassNotNull(message, nameof(message)),
            (object?) null,
            (object?) null
        )
    { }

    public DynamicError()
        : this
        (
            (string?) null,
            (object?) null,
            (object?) null
        )
    { }

    // ReSharper restore RedundantCast

    public override string Message => _message;

    public override IError? InnerError => _innerError;

    public IReadOnlyCollection<IError>? InnerErrors
    {
        get
        {
            if (_innerErrors is null)
            {
                return null;
            }

            if (_innerErrorsRocView is not null)
            {
                return _innerErrorsRocView;
            }

            var rocView = _innerErrors.Length != 0
                ? new ReadOnlyCollection<IError>(_innerErrors)
                : ReadOnlyCollection<IError>.Empty;

            _innerErrorsRocView = rocView;

            return rocView;
        }
    }

    public string? StackTrace => _stackTraceCell.GetStackTrace();

    public StackTrace? LocalStackTrace => _stackTraceCell.GetLocalStackTrace();

}


public class DynamicErrorBuilder(string? message = null)
{
    private object? _errors;
    private object? _traces;

    private static readonly object? PseudoNull = new();

    private DynamicErrorBuilder WithErrorSource(object? errors)
    {
        if (_errors is not null)
        {
            throw new InvalidOperationException("Error already set");
        }
        _errors = errors ?? PseudoNull;
        return this;
    }

    private DynamicErrorBuilder WithTraceSource(object? traces)
    {
        if (_traces is not null)
        {
            throw new InvalidOperationException("Stack trace already set");
        }
        _traces = traces ?? PseudoNull;
        return this;
    }

    public DynamicErrorBuilder WithInnerError(IError? error)
    {
        return WithErrorSource(error);
    }

    public DynamicErrorBuilder WithInnerErrors(IError[]? errors)
    {
        return WithErrorSource(errors);
    }

    public DynamicErrorBuilder WithStackTrace(string? stackTrace)
    {
        return WithTraceSource(stackTrace);
    }

    public DynamicErrorBuilder WithLocalStackTrace(StackTrace? localStackTrace)
    {
        return WithTraceSource(localStackTrace);
    }

    public DynamicError Build()
    {
        var errors = ReferenceEquals(_errors, PseudoNull) ? null : _errors;
        var traces = ReferenceEquals(_traces, PseudoNull) ? null : _traces;
        return new DynamicError(message, errors, traces);
    }
}
