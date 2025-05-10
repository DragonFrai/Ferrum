namespace Ferrum

open System
open System.Diagnostics

open Ferrum.Internal


type AggregateError(message: string, errors: IError seq) =

    interface IError with
        member this.Message =
            message

        member this.InnerError =
            Utils.tryFirst errors

    interface IAggregateError with
        member this.IsAggregate =
            true

        member this.InnerErrors: IError seq =
            errors

    override this.ToString() =
        ErrorFormatter.Default.Format(this)

    interface IFormattable with
        member this.ToString(format, _formatProvider) =
            (ErrorFormatter.byFormat format).Format(this)


type AggregateTracedError(message: string, errors: IError seq, stackTrace: StackTrace) =
    inherit AggregateError(message, errors)

    [<StackTraceHidden>]
    new(message: string, errors: IError seq) = AggregateTracedError(message, errors, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
