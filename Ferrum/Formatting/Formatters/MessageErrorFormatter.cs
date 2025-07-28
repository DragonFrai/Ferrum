namespace Ferrum.Formatting.Formatters;


/// <summary>
/// <para>
/// Formats errors as a single line string with the final error message.
/// (Aggregated errors are not expanded or marked)
/// </para>
/// <para>
/// The <c>Format</c> method returns string like this:
/// <code>
/// Final error
/// </code>
/// </para>
/// </summary>
/// <example>
/// <code>
/// MessageErrorFormatter.Instance.Format(error);
/// error.Format(MessageErrorFormatter.Instance);
/// error.Format("M");
/// error.FormatM();
/// </code>
/// </example>
public class MessageErrorFormatter : IErrorFormatter
{
    private MessageErrorFormatter() { }

    public static readonly MessageErrorFormatter Instance = new MessageErrorFormatter();

    public string Format(IError error)
    {
        return error.Message;
    }
}
