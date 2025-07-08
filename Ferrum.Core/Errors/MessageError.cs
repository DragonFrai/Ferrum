using System.Diagnostics;
using Ferrum.Formatting;

namespace Ferrum.Errors;


public class MessageError(string message) : IError, IFormattable
{
    public string Message => message;

    public IError? InnerError => null;

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return (format is not null ? ErrorFormatter.ByFormat(format) : ErrorFormatter.Default).Format(this);
    }

    public override string ToString()
    {
        return ErrorFormatter.Default.Format(this);
    }
}


public class MessageTracedError(string message, StackTrace stackTrace) : MessageError(message), ITracedError
{
    [StackTraceHidden]
    public MessageTracedError(string message) :
        this(message, new StackTrace(0, true)) {}
    public string? StackTrace => stackTrace.ToString();
    public StackTrace? LocalStackTrace => stackTrace;
}
