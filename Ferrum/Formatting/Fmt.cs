using System.Text;

namespace Ferrum.Formatting;


/// <summary>
/// Internal class for error formatting literals and utils.
/// </summary>
internal static class Fmt
{
    public const string NoMessage = "<NoMessage>";

    public static string MessageOrDefault(IError error)
    {
        var msg = error.Message;
        return string.IsNullOrEmpty(msg) ? NoMessage : msg;
    }
}

internal static class StringBuilderExtensions
{


    public static StringBuilder AppendErrorMessage(this StringBuilder sb, IError error)
    {
        return sb.Append(Fmt.MessageOrDefault(error));
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