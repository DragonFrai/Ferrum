namespace Ferrum

open System.Diagnostics


type MessageError(reason: string) =

    override this.ToString() = reason

    interface IError with
        member this.Reason = reason
        member this.Source = ValueNone


type MessageTracedError(reason: string, stackTrace: StackTrace) =
    inherit MessageError(reason)

    [<StackTraceHidden>]
    new(reason: string) = MessageTracedError(reason, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
