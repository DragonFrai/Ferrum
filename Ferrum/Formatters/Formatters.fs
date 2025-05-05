namespace Ferrum

open System
open System.Text
open Ferrum
open Ferrum.Internal


[<RequireQualifiedAccess>]
module private String =
    let inline notEmptyOr (defaultStr: string) (str: string) : string =
        if String.IsNullOrEmpty(str)
        then defaultStr
        else str

// Error string Builder
[<RequireQualifiedAccess>]
module private SB =

    let [<Literal>] NoReason = "<NoReason>"
    let [<Literal>] Colon = ": "
    let [<Literal>] Error = "Error: "
    let [<Literal>] CausedBy = "Caused by: "
    let [<Literal>] FinalStackTrace = "Final stack trace:"

    let inline create () : StringBuilder =
        StringBuilder()

    let inline appendLine (sb: StringBuilder) : StringBuilder =
        sb.AppendLine()

    let inline toString (sb: StringBuilder) : string =
        sb.ToString()

    let inline notEmptyOrNoReason (reason: string) : string =
        String.notEmptyOr NoReason reason

    let inline reasonLength (error: IError) : int =
        (notEmptyOrNoReason error.Reason).Length

    let inline appendColon (sb: StringBuilder) : StringBuilder =
        sb.Append(Colon)

    let inline appendFinalPrelude (sb: StringBuilder) : StringBuilder =
        sb.Append(Error)

    let inline appendFinalTracePrelude (sb: StringBuilder) : StringBuilder =
        sb.Append(FinalStackTrace)

    let inline appendChainPrelude (sb: StringBuilder) : StringBuilder =
        sb.Append(CausedBy)

    let inline appendReason (error: IError) (sb: StringBuilder) : StringBuilder =
        sb.Append(notEmptyOrNoReason error.Reason)

    let inline appendTraceWithPrelude
            ([<InlineIfLambda>] prelude: StringBuilder -> StringBuilder)
            (error: IError)
            (sb: StringBuilder)
            : StringBuilder =
        let trace = Utils.stackTraceChecked error
        match trace with
        | null -> sb
        | trace -> (prelude sb).Append(trace)

    let inline appendTrace (error: IError) (sb: StringBuilder) : StringBuilder =
        let trace = Utils.stackTraceChecked error
        match trace with
        | null -> sb
        | trace -> sb.Append(trace)

    let inline appendChain<'s>
            ([<InlineIfLambda>] finalAppend: IError -> 's -> 's)
            ([<InlineIfLambda>] chainAppend: IError -> 's -> 's)
            (error: IError)
            (state: 's)
            : 's =
        let mutable state = state
        do state <- finalAppend error state
        do
            let mutable source = error.Source
            while source.IsSome do
                let sourceError = source.Value
                state <- chainAppend sourceError state
                source <- sourceError.Source
        state


type FinalErrorFormatter private () =
    static let _instance = FinalErrorFormatter()
    static member Instance: FinalErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.notEmptyOrNoReason error.Reason

type FinalMultilineErrorFormatter private () =
    static let _instance = FinalMultilineErrorFormatter()
    static member Instance: FinalMultilineErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendFinalPrelude
            |> SB.appendReason error
            |> SB.appendLine
            |> SB.appendTrace error
            |> SB.toString

type ChainErrorFormatter private () =
    static let _instance = ChainErrorFormatter()
    static member Instance: ChainErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendChain
                (fun error sb -> sb |> SB.appendReason error)
                (fun error sb -> sb |> SB.appendColon |> SB.appendReason error)
                error
            |> SB.toString

type ChainMultilineErrorFormatter private () =
    static let _instance = ChainMultilineErrorFormatter()
    static member Instance: ChainMultilineErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendChain
                (fun error sb ->
                    sb
                    |> SB.appendFinalPrelude
                    |> SB.appendReason error
                    |> SB.appendLine
                    |> SB.appendTrace error)
                (fun error sb ->
                    sb
                    |> SB.appendChainPrelude
                    |> SB.appendReason error
                    |> SB.appendLine
                    |> SB.appendTrace error)
                error
            |> SB.toString

type ChainShortenedErrorFormatter private () =
    static let _instance = ChainShortenedErrorFormatter()
    static member Instance: ChainShortenedErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendChain
                (fun error sb ->
                    sb
                    |> SB.appendFinalPrelude
                    |> SB.appendReason error
                    |> SB.appendLine)
                (fun error sb ->
                    sb
                    |> SB.appendChainPrelude
                    |> SB.appendReason error
                    |> SB.appendLine)
                error
            |> SB.appendTraceWithPrelude
                   (fun sb -> sb |> SB.appendFinalTracePrelude |> SB.appendLine)
                   error
            |> SB.toString
