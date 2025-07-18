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

    public static string FormatM(this IError error)
    {
        return error.Format("m");
    }

    public static string FormatS(this IError error)
    {
        return error.Format("s");
    }

    public static string FormatD(this IError error)
    {
        return error.Format("d");
    }

    public static string FormatX(this IError error)
    {
        return error.Format("x");
    }


}
