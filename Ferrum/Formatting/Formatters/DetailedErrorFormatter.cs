using System.Text;

namespace Ferrum.Formatting.Formatters;

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
