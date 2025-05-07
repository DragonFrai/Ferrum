namespace Ferrum

open System.Diagnostics


type MessageError(message: string) =

    override this.ToString() = message

    interface IError with
        member this.Message = message
        member this.InnerError = ValueNone


type MessageTracedError(message: string, stackTrace: StackTrace) =
    inherit MessageError(message)

    [<StackTraceHidden>]
    new(message: string) = MessageTracedError(message, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
