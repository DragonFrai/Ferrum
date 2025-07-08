using System.Diagnostics;
using Ferrum.Formatting;

namespace Ferrum.Errors;



public class WrappedError<T>(T error) : IError, IFormattable
{

    public string Message => Internal.ToStringOrDefault(error);

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

public class WrappedTracedError<T>(T error, StackTrace stackTrace) : WrappedError<T>(error), ITracedError
{
    [StackTraceHidden]
    public WrappedTracedError(T error) :
        this(error, new StackTrace(0, true)) {}
    public string? StackTrace => stackTrace.ToString();
    public StackTrace? LocalStackTrace => stackTrace;
}
