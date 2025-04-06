namespace Ferrum

open System.Collections.Generic
open System.Collections


[<AutoOpen>]
module AggregateExtensions =

    [<RequireQualifiedAccess>]
    module Errors =

        type Aggregate(reason: string, errors: IError seq) =

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

    [<RequireQualifiedAccess>]
    module AggregateError =

        let errors (error: Errors.Aggregate) : IError seq =
            error.Errors


    [<RequireQualifiedAccess>]
    module Error =

        let inline aggregate (reason: string) (errors: IError seq) : Errors.Aggregate =
            Errors.Aggregate(reason, errors)

        let segregate (error: IError) : IError seq =
            match error with
            | :? Errors.Aggregate as error -> error.Errors
            | error -> Seq.singleton error
