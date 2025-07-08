namespace Ferrum.Formatting;


public static class ErrorExtensions
{
    public static string Format(this IError error, IErrorFormatter formatter)
    {
        return formatter.Format(error);
    }
}
