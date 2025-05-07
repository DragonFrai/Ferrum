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

    let [<Literal>] NoMessage = "<NoMessage>"
    let [<Literal>] Colon = ": "
    let [<Literal>] Error = "Error: "
    let [<Literal>] CausedBy = "Cause: "

    let inline create () : StringBuilder =
        StringBuilder()

    let inline appendLine (sb: StringBuilder) : StringBuilder =
        sb.AppendLine()

    let inline toString (sb: StringBuilder) : string =
        sb.ToString()

    let inline notEmptyOrNoMessage (message: string) : string =
        String.notEmptyOr NoMessage message

    let inline appendColon (sb: StringBuilder) : StringBuilder =
        sb.Append(Colon)

    let inline appendFinalPrelude (sb: StringBuilder) : StringBuilder =
        sb.Append(Error)

    let inline appendChainPrelude (sb: StringBuilder) : StringBuilder =
        sb.Append(CausedBy)

    let inline appendMessage (error: IError) (sb: StringBuilder) : StringBuilder =
        sb.Append(notEmptyOrNoMessage error.Message)

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

    let inline appendChainWith<'s>
            ([<InlineIfLambda>] finalAppend: IError -> 's -> 's)
            ([<InlineIfLambda>] chainAppend: IError -> 's -> 's)
            ([<InlineIfLambda>] afterRoot: IError -> 's -> 's)
            (error: IError)
            (state: 's)
            : 's =
        let mutable state = state
        do state <- finalAppend error state
        let mutable error = error
        let mutable innerError = error.InnerError
        while innerError.IsSome do
            let innerErrorValue = innerError.Value
            state <- chainAppend innerErrorValue state
            error <- innerErrorValue
            innerError <- innerErrorValue.InnerError
        state <- afterRoot error state
        state

    let inline appendChain<'s>
            ([<InlineIfLambda>] finalAppend: IError -> 's -> 's)
            ([<InlineIfLambda>] chainAppend: IError -> 's -> 's)
            (error: IError)
            (state: 's)
            : 's =
        appendChainWith finalAppend chainAppend (fun _e s -> s) error state


type FinalMessageErrorFormatter private () =
    static let _instance = FinalMessageErrorFormatter()
    static member Instance: FinalMessageErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.notEmptyOrNoMessage error.Message

type ChainMessageErrorFormatter private () =
    static let _instance = ChainMessageErrorFormatter()
    static member Instance: ChainMessageErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendChain
                (fun error sb -> sb |> SB.appendMessage error)
                (fun error sb -> sb |> SB.appendColon |> SB.appendMessage error)
                error
            |> SB.toString

type FinalErrorFormatter private () =
    static let _instance = FinalErrorFormatter()
    static member Instance: FinalErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendFinalPrelude
            |> SB.appendMessage error
            |> SB.appendLine
            |> SB.appendTrace error
            |> SB.toString

type ChainErrorFormatter private () =
    static let _instance = ChainErrorFormatter()
    static member Instance: ChainErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendChainWith
                (fun error sb ->
                    sb
                    |> SB.appendFinalPrelude
                    |> SB.appendMessage error
                    |> SB.appendLine)
                (fun error sb ->
                    sb
                    |> SB.appendChainPrelude
                    |> SB.appendMessage error
                    |> SB.appendLine)
                (fun error sb ->
                    sb |> SB.appendTrace error)
                error
            |> SB.toString

type TraceErrorFormatter private () =
    static let _instance = TraceErrorFormatter()
    static member Instance: TraceErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendChain
                (fun error sb ->
                    sb
                    |> SB.appendFinalPrelude
                    |> SB.appendMessage error
                    |> SB.appendLine
                    |> SB.appendTrace error)
                (fun error sb ->
                    sb
                    |> SB.appendChainPrelude
                    |> SB.appendMessage error
                    |> SB.appendLine
                    |> SB.appendTrace error)
                error
            |> SB.toString
