using System.Text;

namespace Ferrum.Formatting.Formatters;

public class SummaryErrorFormatter : IErrorFormatter
{
    private SummaryErrorFormatter() { }

    public static readonly SummaryErrorFormatter Instance = new SummaryErrorFormatter();

    public string Format(IError error)
    {
        var sb = new StringBuilder();
        sb = Fmt.FoldChain(
            (s, err) => s.AppendErrorMessage(err),
            (s, err) => s.AppendLiteralColon().AppendErrorMessage(err),
            error,
            sb
        );
        return sb.ToString();
    }
}
