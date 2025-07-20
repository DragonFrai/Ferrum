namespace Ferrum.Formatting.Formatters;

using System.Text;


/// <summary>
/// <para>
/// Formats errors as a multi-line string listing the final error and all its inner errors with the ordinal number.
/// After all errors, all available stack traces are added with the corresponding error numbers.
/// (Aggregated errors are not expanded or marked)
/// (The string is expected to end with a line break)
/// </para>
/// <para>
/// The <c>Format</c> method returns string like this:
/// <code>
/// [0] Error: Final error
/// [1] Cause: Middle error
/// [2] Cause: Root error
/// Trace [0]:
///    at ... (stack trace)
/// Trace [2]:
///    at ... (stack trace)
/// </code>
/// </para>
/// </summary>
/// <example>
/// <code>
/// DiagnosticErrorFormatter.Instance.Format(error);
/// error.Format(DiagnosticErrorFormatter.Instance);
/// error.Format("X");
/// error.FormatX();
/// </code>
/// </example>
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