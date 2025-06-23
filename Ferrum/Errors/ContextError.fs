namespace Ferrum

open System
open System.Diagnostics


type ContextError(context: string, innerError: IError) =
    
    do if innerError = null then invalidArg "innerError" "Error is null"

    interface IError with
        member this.Message = context
        member this.InnerError = innerError

    override this.ToString() =
        ErrorFormatter.Default.Format(this)

    interface IFormattable with
        member this.ToString(format, _formatProvider) =
            (ErrorFormatter.byFormat format).Format(this)


type ContextTracedError(context: string, source: IError, stackTrace: StackTrace) =
    inherit ContextError(context, source)

    [<StackTraceHidden>]
    new(message: string, source: IError) = ContextTracedError(message, source, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
