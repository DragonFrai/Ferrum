namespace Ferrum.Formatting;

public static class FormattersErrorExtensions
{
    public static string Format(this IError error, string format)
    {
        return ErrorFormatter.ByFormat(format).Format(error);
    }

    public static string Format(this IError error, int level)
    {
        return ErrorFormatter.ByLevel(level).Format(error);
    }

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.MessageErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    public static string FormatM(this IError error)
    {
        return error.Format("m");
    }

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.SummaryErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    public static string FormatS(this IError error)
    {
        return error.Format("s");
    }

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.DetailedErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    public static string FormatD(this IError error)
    {
        return error.Format("d");
    }

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.DiagnosticErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    public static string FormatX(this IError error)
    {
        return error.Format("x");
    }


}
