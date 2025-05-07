namespace Ferrum

open System.Diagnostics


type ContextError(context: string, source: IError) =

    override this.ToString() = context

    interface IError with
        member this.Message = context
        member this.InnerError = ValueSome source


type ContextTracedError(context: string, source: IError, stackTrace: StackTrace) =
    inherit ContextError(context, source)

    [<StackTraceHidden>]
    new(message: string, source: IError) = ContextTracedError(message, source, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
