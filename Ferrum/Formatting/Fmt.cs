using System.Text;

namespace Ferrum.Formatting;



internal static class StringBuilderExtensions
{
    private const string NoMessage = "<NoMessage>";

    public static StringBuilder AppendErrorMessage(this StringBuilder sb, IError error)
    {
        var msg = error.Message;
        var s = string.IsNullOrEmpty(msg) ? NoMessage : msg;
        return sb.Append(s);
    }

    public static StringBuilder AppendErrorTrace(this StringBuilder sb, string? stackTraceStr)
    {
        if (string.IsNullOrEmpty(stackTraceStr))
        {
            return sb;
        }

        if (!stackTraceStr.StartsWith("   at"))
        {
            sb.Append("   at");
        }
        sb.Append(stackTraceStr);
        if (!(
                stackTraceStr.EndsWith(Environment.NewLine)
                || stackTraceStr.EndsWith('\n')
                || stackTraceStr.EndsWith('\r')
                || stackTraceStr.EndsWith("\r\n")
            ))
        {
            sb.AppendLine();
        }

        return sb;
    }
}