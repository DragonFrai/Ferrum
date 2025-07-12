using System.Diagnostics;

namespace Ferrum.Errors;


public class TracedContextError : ContextError, ITracedError
{
    [StackTraceHidden]
    public TracedContextError(string message, IError innerError) : base(message, innerError)
    {
        var localStackTrace = new StackTrace(0, true);
        LocalStackTrace = localStackTrace;
        StackTrace = localStackTrace.ToString();
    }

    public string StackTrace { get; }

    public StackTrace LocalStackTrace { get; }
}
