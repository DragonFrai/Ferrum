namespace Ferrum


[<AutoOpen>]
module FundamentalExtensions =

    type IError with
        member this.Chain() : IError seq =
            let mutable current = ValueSome this
            seq {
                match current with
                | ValueNone -> ()
                | ValueSome err ->
                    current <- err.Source
                    yield err
            }

        member this.GetRoot() : IError =
            this.Chain() |> Seq.last

    [<RequireQualifiedAccess>]
    module Error =

        let inline chain (err: IError) : IError seq =
            err.Chain()

        let inline getRoot (err: IError) : IError =
            err.GetRoot()
