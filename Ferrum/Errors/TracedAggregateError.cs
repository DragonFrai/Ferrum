using System.Diagnostics;

namespace Ferrum.Errors;


public class TracedAggregateError : AggregateError, ITracedError
{
    private readonly StackTrace? _localStackTrace;
    private readonly string? _stackTrace;

    [StackTraceHidden]
    public TracedAggregateError(string message, IError[] innerErrors)
        : base(message, innerErrors)
    {
        var localStackTrace = new StackTrace(0, true);
        _localStackTrace = localStackTrace;
        _stackTrace = localStackTrace.ToString();
    }

    [StackTraceHidden]
    public TracedAggregateError(string message, IEnumerable<IError> innerErrors)
        : base(message, innerErrors)
    {
        var localStackTrace = new StackTrace(0, true);
        _localStackTrace = localStackTrace;
        _stackTrace = localStackTrace.ToString();
    }

    [StackTraceHidden]
    public TracedAggregateError(string message) : base(message)
    {
        var localStackTrace = new StackTrace(0, true);
        _localStackTrace = localStackTrace;
        _stackTrace = localStackTrace.ToString();
    }

    [StackTraceHidden]
    public TracedAggregateError() : base()
    {
        var localStackTrace = new StackTrace(0, true);
        _localStackTrace = localStackTrace;
        _stackTrace = localStackTrace.ToString();
    }

    public string? StackTrace => _stackTrace;

    public StackTrace? LocalStackTrace => _localStackTrace;
}
