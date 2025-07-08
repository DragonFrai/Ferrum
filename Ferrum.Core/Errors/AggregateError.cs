using System.Collections.ObjectModel;
using System.Diagnostics;
using Ferrum.Formatting;

namespace Ferrum.Errors;

public class AggregateError(string message, IEnumerable<IError> innerErrors) : IError, IAggregateError, IFormattable
{

    public string Message => message;

    public IError? InnerError => null;

    public bool IsAggregate => true;

    public IEnumerable<IError> InnerErrors => innerErrors;

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return (format is not null ? ErrorFormatter.ByFormat(format) : ErrorFormatter.Default).Format(this);
    }

    public override string ToString()
    {
        return ErrorFormatter.Default.Format(this);
    }
}


public class AggregateTracedError(string message, IEnumerable<IError> innerErrors, StackTrace stackTrace)
    : AggregateError(message, innerErrors), ITracedError
{
    [StackTraceHidden]
    public AggregateTracedError(string message, IEnumerable<IError> innerErrors) :
        this(message, innerErrors, new StackTrace(0, true)) {}
    public string? StackTrace => stackTrace.ToString();
    public StackTrace? LocalStackTrace => stackTrace;
}
