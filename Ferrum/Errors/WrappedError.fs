namespace Ferrum

open System.Diagnostics


type WrappedError<'e>(error: 'e) =

    override this.ToString() = $"{error}"

    interface IError with
        member this.Message = $"{error}"
        member this.InnerError = ValueNone


type WrappedTracedError<'e>(error: 'e, stackTrace: StackTrace) =
    inherit WrappedError<'e>(error)

    [<StackTraceHidden>]
    new(error: 'e) = WrappedTracedError(error, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
