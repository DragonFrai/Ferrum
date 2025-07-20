using System.Text;

namespace Ferrum.Formatting.Formatters;


/// <summary>
/// <para>
/// Formats errors as a single line string of error chain messages separated by ':'.
/// (Aggregated errors are not expanded or marked)
/// </para>
/// <para>
/// The <c>Format</c> method returns string like this:
/// <code>
/// Final error: Middle error: Root error
/// </code>
/// </para>
/// </summary>
/// <example>
/// <code>
/// SummaryErrorFormatter.Instance.Format(error);
/// error.Format(SummaryErrorFormatter.Instance);
/// error.Format("S");
/// error.FormatS();
/// </code>
/// </example>
public class SummaryErrorFormatter : IErrorFormatter
{
    private SummaryErrorFormatter() { }

    public static readonly SummaryErrorFormatter Instance = new SummaryErrorFormatter();

    public string Format(IError error)
    {
        var sb = new StringBuilder();

        sb.AppendErrorMessage(error);

        IError? current = error.InnerError;
        while (current is not null)
        {
            sb.Append(": ").AppendErrorMessage(current);
            current = current.InnerError;
        }

        return sb.ToString();
    }
}
