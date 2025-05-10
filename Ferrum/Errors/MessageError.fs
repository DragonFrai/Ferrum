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


type MessageTracedError(message: string, stackTrace: StackTrace) =
    inherit MessageError(message)

    [<StackTraceHidden>]
    new(message: string) = MessageTracedError(message, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
