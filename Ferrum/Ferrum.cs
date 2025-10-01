using System.Diagnostics;
using Ferrum.Errors;

namespace Ferrum;


public enum TraceMode
{
    Never,
    Once,
    Always
}


public static class Ferrum
{
    public static class Global
    {
        private enum ConfigurationTraceMode
        {
            Never = TraceMode.Never,
            Once = TraceMode.Once,
            Always = TraceMode.Always,
            NotConfigured = 3
        }

        private static uint _traceMode = (int) ConfigurationTraceMode.NotConfigured;

        private const TraceMode DEFAULT_TRACE_MODE = TraceMode.Once;

        public static TraceMode TraceMode
        {
            get
            {
                var ctm = _traceMode;

                if ((ConfigurationTraceMode) ctm != ConfigurationTraceMode.NotConfigured)
                {
                    return (TraceMode) ctm;
                }

                var exchanged = Interlocked.CompareExchange(ref _traceMode, (uint) DEFAULT_TRACE_MODE, ctm);

                if ((ConfigurationTraceMode) exchanged == (ConfigurationTraceMode) ctm)
                {
                    return DEFAULT_TRACE_MODE;
                }

                return (TraceMode) exchanged;
            }
        }

        public static void Init(TraceMode traceMode)
        {
            var exchanged = Interlocked.CompareExchange(ref _traceMode, (uint) traceMode, (uint) ConfigurationTraceMode.NotConfigured);
            if ((ConfigurationTraceMode)exchanged != ConfigurationTraceMode.NotConfigured)
            {
                throw new InvalidOperationException("Ferrum default TraceMode can be set only once");
            }
        }

        public static void InitDefault()
        {
            Init(DEFAULT_TRACE_MODE);
        }

        public static void InitEnvironment()
        {
            var traceModeStr = Environment.GetEnvironmentVariable("FERRUM_TRACE_MODE");
            TraceMode traceMode;
            if (traceModeStr is null)
            {
                traceMode = DEFAULT_TRACE_MODE;
            }
            else if (string.Equals(traceModeStr, "never", StringComparison.InvariantCultureIgnoreCase))
            {
                traceMode = TraceMode.Never;
            }
            else if (string.Equals(traceModeStr, "once", StringComparison.InvariantCultureIgnoreCase))
            {
                traceMode = TraceMode.Once;
            }
            else if (string.Equals(traceModeStr, "always", StringComparison.InvariantCultureIgnoreCase))
            {
                traceMode = TraceMode.Always;
            }
            else
            {
                traceMode = DEFAULT_TRACE_MODE;
            }
            Init(traceMode);
        }
    }

    public static IError Message(string message, bool stackTrace)
    {
        return !stackTrace ? new MessageError(message) : new TracedMessageError(message);
    }

    public static IError Message(string message, TraceMode traceMode)
    {
        return Message(message, traceMode != TraceMode.Never);
    }

    public static IError Message(string message)
    {
        return Message(message, Global.TraceMode);
    }

    public static IError Context(string message, IError innerError, TraceMode traceMode)
    {
        var traced = traceMode switch
        {
            TraceMode.Never => false,
            TraceMode.Once => !innerError.IsChainHasStakTrace(),
            TraceMode.Always => true,
            _ => throw new UnreachableException()
        };
        return traced ? new TracedContextError(message, innerError) : new ContextError(message, innerError);
    }

    public static IError Context(string message, IError innerError)
    {
        return Context(message, innerError, Global.TraceMode);
    }
}

