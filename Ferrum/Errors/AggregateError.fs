namespace Ferrum

open System.Collections.Generic
open System.Collections
open System.Diagnostics


type AggregateError(reason: string, errors: IError seq) =

    member this.Errors: IError seq = errors

    interface IError with
        member this.Reason = reason
        member this.Source =
            match errors with
            | :? IReadOnlyList<IError> as errors ->
                if errors.Count > 0 then ValueSome errors[0] else ValueNone
            | :? IList<IError> as errors ->
                if errors.Count > 0 then ValueSome errors[0] else ValueNone
            | :? IList as errors ->
                if errors.Count > 0 then ValueSome (errors[0] :?> IError) else ValueNone
            | errors ->
                use e = errors.GetEnumerator()
                if e.MoveNext() then ValueSome e.Current else ValueNone

type AggregateTracedError(reason: string, errors: IError seq, stackTrace: StackTrace) =
    inherit AggregateError(reason, errors)

    [<StackTraceHidden>]
    new(reason: string, errors: IError seq) = AggregateTracedError(reason, errors, StackTrace(0, true))

    interface ITracedError with
        member this.StackTrace = stackTrace.ToString()
        member this.LocalStackTrace = stackTrace


[<RequireQualifiedAccess>]
module AggregateError =

    let errors (error: AggregateError) : IError seq =
        error.Errors
