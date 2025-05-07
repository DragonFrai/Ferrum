namespace Ferrum

open System
open System.Diagnostics


type WrappedError<'e>(error: 'e) =

    interface IError with
        member this.Message = $"{error}"
        member this.InnerError = ValueNone

    override this.ToString() =
        ErrorFormatter.Default.Format(this)

    interface IFormattable with
        member this.ToString(format, _formatProvider) =
            (ErrorFormatter.byFormat format).Format(this)


type WrappedTracedError<'e>(error: 'e, stackTrace: StackTrace) =
    inherit WrappedError<'e>(error)

    [<StackTraceHidden>]
    new(error: 'e) = WrappedTracedError(error, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
