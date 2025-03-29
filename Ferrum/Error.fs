namespace Ferrum


[<AutoOpen>]
module FundamentalExtensions =

    type IError with
        member this.Chain() : IError seq =
            seq {
                let rec loop (current: IError voption) = seq {
                    match current with
                    | ValueNone -> ()
                    | ValueSome err ->
                        yield err
                        yield! loop err.Source
                }
                yield! loop (ValueSome this)
            }

        member this.GetRoot() : IError =
            this.Chain() |> Seq.last

    [<RequireQualifiedAccess>]
    module Error =

        let inline chain (err: IError) : IError seq =
            err.Chain()

        let inline getRoot (err: IError) : IError =
            err.GetRoot()
