using Ferrum.Formatting;

namespace Ferrum.Errors;

public abstract class BaseError : IError, IFormattable
{
    public abstract string Message { get; }
    public abstract IError? InnerError { get; }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return (format is not null ? ErrorFormatter.ByFormat(format) : ErrorFormatter.Default).Format(this);
    }

    public override string ToString()
    {
        return ErrorFormatter.Default.Format(this);
    }
}
