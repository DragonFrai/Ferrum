using System.Diagnostics;

namespace Ferrum.Errors;


public class TracedAggregateError : AggregateError, ITracedError
{
    private StackTrace? _localStackTrace;
    private string? _stackTrace;

    [StackTraceHidden]
    private static void InitStackTrace(TracedAggregateError error)
    {
        var localStackTrace = new StackTrace(0, true);
        error._localStackTrace = localStackTrace;
        error._stackTrace = localStackTrace.ToString();
    }

    [StackTraceHidden]
    public TracedAggregateError(string message, IReadOnlyList<IError> innerErrors, bool cloneInnerErrors = true)
        : base(message, innerErrors, cloneInnerErrors)
    {
        InitStackTrace(this);
    }

    [StackTraceHidden]
    public TracedAggregateError(string message, IEnumerable<IError> innerErrors)
        : base(message, innerErrors)
    {
        InitStackTrace(this);
    }

    [StackTraceHidden]
    public TracedAggregateError(string message) : base(message)
    {
        InitStackTrace(this);
    }

    [StackTraceHidden]
    public TracedAggregateError() : base()
    {
        InitStackTrace(this);
    }

    public string? StackTrace => _stackTrace;

    public StackTrace? LocalStackTrace => _localStackTrace;
}
