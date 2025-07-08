namespace Ferrum.Formatting.Formatters;

using System.Text;

public class DiagnosticErrorFormatter : IErrorFormatter
{
    private DiagnosticErrorFormatter() { }

    public static readonly DiagnosticErrorFormatter Instance = new DiagnosticErrorFormatter();

    public string Format(IError error)
    {
        var sb = new StringBuilder();
        Fmt.FoldChain(
            (s, err) =>
                s.AppendLiteralError()
                    .AppendErrorMessage(err)
                    .AppendLine()
                    .AppendErrorTrace(err),
            (s, err) =>
                s.AppendLiteralCause()
                    .AppendErrorMessage(err)
                    .AppendLine()
                    .AppendErrorTrace(err),
            error,
            sb
        );
        return sb.ToString();
    }
}