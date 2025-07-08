using System.Globalization;

namespace Ferrum;

internal static class Internal
{
    private static readonly string NullLiteral = string.Empty;

    public static string ToStringOrDefault<T>(T value)
    {
        return value switch
        {
            IConvertible convertible => convertible.ToString(CultureInfo.InvariantCulture) ?? NullLiteral,
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? NullLiteral,
            null => NullLiteral,
            _ => value.ToString() ?? NullLiteral
        };
    }

    public static T? TryFirst<T>(IEnumerable<T> collection)
    {
        return collection.FirstOrDefault();
    }
}
