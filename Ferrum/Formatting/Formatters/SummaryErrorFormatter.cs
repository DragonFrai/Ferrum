using System.Text;

namespace Ferrum.Formatting.Formatters;

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
