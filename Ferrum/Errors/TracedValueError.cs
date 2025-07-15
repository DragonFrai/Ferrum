using System.Diagnostics;

namespace Ferrum.Errors;

public class TracedValueError<T> : ValueError<T>, ITracedError
{
    [StackTraceHidden]
    public TracedValueError(T value, Func<T, string>? toMessage = null) : base(value, toMessage)
    {
        var localStackTrace = new StackTrace(0, true);
        LocalStackTrace = localStackTrace;
        StackTrace = localStackTrace.ToString();
    }

    public string StackTrace { get; }

    public StackTrace LocalStackTrace { get; }
}
