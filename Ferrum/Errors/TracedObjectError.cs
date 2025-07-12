using System.Diagnostics;

namespace Ferrum.Errors;

public class TracedObjectError<T> : ObjectError<T>, ITracedError
{
    [StackTraceHidden]
    public TracedObjectError(T error, Func<T, string>? toString = null) : base(error, toString)
    {
        var localStackTrace = new StackTrace(0, true);
        LocalStackTrace = localStackTrace;
        StackTrace = localStackTrace.ToString();
    }

    public string StackTrace { get; }

    public StackTrace LocalStackTrace { get; }
}
