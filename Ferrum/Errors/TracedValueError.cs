using System.Diagnostics;

namespace Ferrum.Errors;

public class TracedValueError<T> : ValueError<T>, ITracedError
{
    private StackTraceCell _stackTraceCell;

    [StackTraceHidden]
    // ReSharper disable once ConvertToPrimaryConstructor
    public TracedValueError(T value, Func<T, string>? toMessage = null) : base(value, toMessage)
    {
        _stackTraceCell = new StackTraceCell();
    }

    public string? StackTrace => _stackTraceCell.GetStackTrace();

    public StackTrace? LocalStackTrace => _stackTraceCell.GetLocalStackTrace();
}
