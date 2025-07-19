namespace Ferrum.Formatting.Formatters;

using System.Text;

public class DiagnosticErrorFormatter : IErrorFormatter
{
    private DiagnosticErrorFormatter() { }

    public static readonly DiagnosticErrorFormatter Instance = new DiagnosticErrorFormatter();

    public string Format(IError error)
    {
        var sb = new StringBuilder();

        IError? current = error;
        int index = 0;
        while (current is not null)
        {
            if (index == 0)
            {
                sb.Append('[').Append(index).Append("] Error: ").AppendErrorMessage(current).AppendLine();
            }
            else
            {
                sb.Append('[').Append(index).Append("] Cause: ").AppendErrorMessage(current).AppendLine();
            }

            index++;
            current = current.InnerError;
        }

        current = error;
        index = 0;
        while (current is not null)
        {
            if (current is ITracedError { HasStackTrace: true } tracedCurrent)
            {
                sb.Append("Trace [").Append(index).Append("]:").AppendLine().AppendErrorTrace(tracedCurrent.StackTrace);
            }

            index++;
            current = current.InnerError;
        }

        return sb.ToString();
    }
}