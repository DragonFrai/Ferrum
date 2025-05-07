namespace Ferrum

open System
open System.Diagnostics


type MessageError(message: string) =

    interface IError with
        member this.Message = message
        member this.InnerError = ValueNone

    override this.ToString() =
        ErrorFormatter.Default.Format(this)

    interface IFormattable with
        member this.ToString(format, _formatProvider) =
            (ErrorFormatter.byFormat format).Format(this)


type MessageTracedError(reason: string, stackTrace: StackTrace) =
    inherit MessageError(reason)

    [<StackTraceHidden>]
    new(reason: string) = MessageTracedError(reason, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
