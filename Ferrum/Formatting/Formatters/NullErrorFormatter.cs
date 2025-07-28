namespace Ferrum.Formatting.Formatters;

/// <summary>
/// Formats errors to empty string.
/// </summary>
public class NullErrorFormatter : IErrorFormatter
{
    private NullErrorFormatter() {}

    public static readonly NullErrorFormatter Instance = new();

    public string Format(IError error)
    {
        return string.Empty;
    }
}
