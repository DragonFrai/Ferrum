namespace Ferrum

open System.Diagnostics


type ContextError(context: string, source: IError) =

    override this.ToString() = context

    interface IError with
        member this.Reason = context
        member this.Source = ValueSome source


type ContextTracedError(context: string, source: IError, stackTrace: StackTrace) =
    inherit ContextError(context, source)

    [<StackTraceHidden>]
    new(reason: string, source: IError) = ContextTracedError(reason, source, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
