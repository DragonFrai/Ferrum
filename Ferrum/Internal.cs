using System.Globalization;

namespace Ferrum;

internal static class Internal
{
    private static readonly string NullLiteral = string.Empty;



    public static T? TryFirst<T>(IEnumerable<T> collection)
    {
        return collection.FirstOrDefault();
    }
}
