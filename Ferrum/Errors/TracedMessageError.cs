using System.Diagnostics;

namespace Ferrum.Errors;

public class TracedMessageError : MessageError, ITracedError
{
    private StackTraceCell _stackTraceCell;

    [StackTraceHidden]
    // ReSharper disable once ConvertToPrimaryConstructor
    public TracedMessageError(string message) : base(message)
    {
        _stackTraceCell = new StackTraceCell();
    }

    public string? StackTrace => _stackTraceCell.GetStackTrace();

    public StackTrace? LocalStackTrace => _stackTraceCell.GetLocalStackTrace();
}
