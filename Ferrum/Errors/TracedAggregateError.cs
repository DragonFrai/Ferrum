using System.Diagnostics;

namespace Ferrum.Errors;


public class TracedAggregateError : AggregateError, ITracedError
{
    private StackTraceCell _stackTraceCell;

    [StackTraceHidden]
    public TracedAggregateError(string message, IError[] innerErrors)
        : base(message, innerErrors)
    {
        _stackTraceCell = new StackTraceCell();
    }

    [StackTraceHidden]
    public TracedAggregateError(string message, IEnumerable<IError> innerErrors)
        : base(message, innerErrors)
    {
        _stackTraceCell = new StackTraceCell();
    }

    [StackTraceHidden]
    public TracedAggregateError(string message) : base(message)
    {
        _stackTraceCell = new StackTraceCell();
    }

    [StackTraceHidden]
    // ReSharper disable once RedundantBaseConstructorCall
    public TracedAggregateError() : base()
    {
        _stackTraceCell = new StackTraceCell();
    }

    public string? StackTrace => _stackTraceCell.GetStackTrace();

    public StackTrace? LocalStackTrace => _stackTraceCell.GetLocalStackTrace();
}
