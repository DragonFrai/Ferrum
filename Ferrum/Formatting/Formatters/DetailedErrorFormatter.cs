using System.Text;

namespace Ferrum.Formatting.Formatters;


/// <summary>
/// <para>
/// Formats errors as a multi-line string listing the final error and all its inner errors with the ordinal number.
/// After all errors, the stack trace of the deepest (not necessarily the root) error is added with its number.
/// (Aggregated errors are not expanded or marked)
/// (The string is expected to end with a line break)
/// </para>
/// <para>
/// The <c>Format</c> method returns string like this:
/// <code>
/// [0] Error: Final error
/// [1] Cause: Middle error
/// [2] Cause: Root error
/// Trace [1]:
///    at ... (stack trace)
/// </code>
/// </para>
/// </summary>
/// <example>
/// <code>
/// DetailedErrorFormatter.Instance.Format(error);
/// error.Format(DetailedErrorFormatter.Instance);
/// error.Format("D");
/// error.FormatD();
/// </code>
/// </example>
public class DetailedErrorFormatter : IErrorFormatter
{
    private DetailedErrorFormatter() { }

    public static readonly DetailedErrorFormatter Instance = new DetailedErrorFormatter();

    public string Format(IError error)
    {
        var sb = new StringBuilder();

        IError? current = error;
        int index = 0;
        ITracedError? lastTraced = null;
        int lastTracedIndex = -1;
        while (current is not null)
        {
            if (current is ITracedError { HasStackTrace: true } tracedCurrent)
            {
                lastTracedIndex = index;
                lastTraced = tracedCurrent;
            }

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

        if (lastTraced is not null)
        {
            sb.Append("Trace [").Append(lastTracedIndex).Append("]:").AppendLine()
                .AppendErrorTrace(lastTraced.StackTrace);
        }

        return sb.ToString();
    }
}
