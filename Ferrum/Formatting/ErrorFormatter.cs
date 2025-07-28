namespace Ferrum.Formatting;

using Formatters;

public static class ErrorFormatter
{
    public static readonly IErrorFormatter Default = SummaryErrorFormatter.Instance;

    public static IErrorFormatter ByFormat(string format)
    {
        return format switch
        {
            null or "" or "G" => Default,
            "m" or "M" or "l1" or "L1" => MessageErrorFormatter.Instance,
            "s" or "S" or "l2" or "L2" => SummaryErrorFormatter.Instance,
            "d" or "D" or "l3" or "L3" => DetailedErrorFormatter.Instance,
            "x" or "X" or "l4" or "L4" => DiagnosticErrorFormatter.Instance,
            "n" or "N" or "l0" or "L0" => NullErrorFormatter.Instance,
            _ => throw new FormatException($"The {format} format string is not supported.")
        };
    }

    public static IErrorFormatter ByLevel(int level)
    {
        return level switch
        {
            1 => MessageErrorFormatter.Instance,
            2 => SummaryErrorFormatter.Instance,
            3 => DetailedErrorFormatter.Instance,
            4 => DiagnosticErrorFormatter.Instance,
            0 => NullErrorFormatter.Instance,
            _ => throw new FormatException($"The {level} format level is not supported.")
        };
    }
}
