namespace Ferrum

open System.Diagnostics

open Ferrum.Internal


type AggregateError(reason: string, errors: IError seq) =

    interface IError with
        member this.Reason =
            reason

        member this.Source =
            Utils.tryFirst errors

    interface IAggregateError with
        member this.IsAggregate =
            true

        member this.Sources: IError seq =
            errors

type AggregateTracedError(reason: string, errors: IError seq, stackTrace: StackTrace) =
    inherit AggregateError(reason, errors)

    [<StackTraceHidden>]
    new(reason: string, errors: IError seq) = AggregateTracedError(reason, errors, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace
