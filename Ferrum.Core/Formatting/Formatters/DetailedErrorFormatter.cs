using System.Text;

namespace Ferrum.Formatting.Formatters;

public class DetailedErrorFormatter : IErrorFormatter
{
    private DetailedErrorFormatter() { }

    public static readonly DetailedErrorFormatter Instance = new DetailedErrorFormatter();

    public string Format(IError error)
    {
        var sb = new StringBuilder();
        Fmt.FoldChainWith(
            (s, err) =>
                s.AppendLiteralError()
                    .AppendErrorMessage(err)
                    .AppendLine(),
            (s, err) =>
                s.AppendLiteralCause()
                    .AppendErrorMessage(err)
                    .AppendLine(),
            (s, err) =>
                s.AppendErrorTrace(err),
            error,
            sb
        );
        return sb.ToString();
    }
}
