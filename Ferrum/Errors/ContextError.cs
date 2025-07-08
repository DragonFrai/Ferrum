using System.Diagnostics;
using Ferrum.Formatting;

namespace Ferrum.Errors;

public class ContextError(string message, IError innerError) : IError, IFormattable
{
    public string Message { get; } = message;

    public IError? InnerError { get; } = innerError ?? throw new ArgumentNullException(nameof(innerError));

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return (format is not null ? ErrorFormatter.ByFormat(format) : ErrorFormatter.Default).Format(this);
    }

    public override string ToString()
    {
        return ErrorFormatter.Default.Format(this);
    }
}


public class ContextTracedError(string message, IError innerError, StackTrace stackTrace) :
    ContextError(message, innerError), ITracedError
{
    [StackTraceHidden]
    public ContextTracedError(string message, IError innerError) :
        this(message, innerError, new StackTrace(0, true)) { }
    public string? StackTrace => stackTrace.ToString();
    public StackTrace? LocalStackTrace => stackTrace;
}
