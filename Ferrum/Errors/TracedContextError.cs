using System.Diagnostics;

namespace Ferrum.Errors;


public class TracedContextError : ContextError, ITracedError
{
    private StackTraceCell _stackTraceCell;

    [StackTraceHidden]
    // ReSharper disable once ConvertToPrimaryConstructor
    public TracedContextError(string message, IError innerError) : base(message, innerError)
    {
        _stackTraceCell = new StackTraceCell();
    }

    public string? StackTrace => _stackTraceCell.GetStackTrace();
}
