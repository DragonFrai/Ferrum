using System.Diagnostics;

namespace Ferrum.Errors;

public class TracedMessageError : MessageError, ITracedError
{
    [StackTraceHidden]
    public TracedMessageError(string message) : base(message)
    {
        var localStackTrace = new StackTrace(0, true);
        LocalStackTrace = localStackTrace;
        StackTrace = localStackTrace.ToString();
    }

    public string StackTrace { get; }

    public StackTrace LocalStackTrace { get; }
}
