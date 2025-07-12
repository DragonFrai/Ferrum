using System.Text;

namespace Ferrum.Formatting;


/// <summary>
/// Internal class for error formatting literals and utils.
/// </summary>
internal static class Fmt
{
    public static string NoMessage = "<NoMessage>";
    public static string Colon = ": ";
    public static string Error = "Error: ";
    public static string Cause = "Cause: ";

    public static string MessageOrDefault(IError error)
    {
        var msg = error.Message;
        return string.IsNullOrEmpty(msg) ? NoMessage : msg;
    }

    public static TState FoldChainWith<TState>(
        Func<TState, IError, TState> finalFolder,
        Func<TState, IError, TState> chainFolder,
        Func<TState, IError, TState> rootPostFolder,
        IError error,
        TState state)
    {
        state = finalFolder(state, error);
        var prev = error;
        var current = error.InnerError;
        while (current is not null)
        {
            state = chainFolder(state, current);
            prev = current;
            current = current.InnerError;
        }
        state = rootPostFolder(state, prev);
        return state;
    }

    public static TState FoldChain<TState>(
        Func<TState, IError, TState> finalFolder,
        Func<TState, IError, TState> chainFolder,
        IError error,
        TState state)
    {
        state = finalFolder(state, error);
        var current = error.InnerError;
        while (current is not null)
        {
            state = chainFolder(state, current);
            current = current.InnerError;
        }
        return state;
    }

}

internal static class StringBuilderExtensions
{

    public static StringBuilder AppendLiteralColon(this StringBuilder sb) => sb.Append(Fmt.Colon);
    public static StringBuilder AppendLiteralError(this StringBuilder sb) => sb.Append(Fmt.Error);
    public static StringBuilder AppendLiteralCause(this StringBuilder sb) => sb.Append(Fmt.Cause);

    public static StringBuilder AppendErrorMessage(this StringBuilder sb, IError error)
    {
        return sb.Append(Fmt.MessageOrDefault(error));
    }

    public static StringBuilder AppendErrorTrace(this StringBuilder sb, IError error)
    {
        var stackTraceStr = error.GetStackTrace();
        if (String.IsNullOrEmpty(stackTraceStr))
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