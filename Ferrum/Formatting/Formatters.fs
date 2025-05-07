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
    let [<Literal>] CausedBy = "Caused by: "
    let [<Literal>] FinalStackTrace = "Final stack trace:"

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

    let inline appendFinalTracePrelude (sb: StringBuilder) : StringBuilder =
        sb.Append(FinalStackTrace)

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

    let inline appendChain<'s>
            ([<InlineIfLambda>] finalAppend: IError -> 's -> 's)
            ([<InlineIfLambda>] chainAppend: IError -> 's -> 's)
            (error: IError)
            (state: 's)
            : 's =
        let mutable state = state
        do state <- finalAppend error state
        do
            let mutable source = error.InnerError
            while source.IsSome do
                let sourceError = source.Value
                state <- chainAppend sourceError state
                source <- sourceError.InnerError
        state


type FinalMessageErrorFormatter private () =
    static let _instance = FinalMessageErrorFormatter()
    static member Instance: FinalMessageErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.notEmptyOrNoMessage error.Message

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

type ChainErrorFormatter private () =
    static let _instance = ChainErrorFormatter()
    static member Instance: ChainErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            SB.create ()
            |> SB.appendChain
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
                error
            |> SB.appendTraceWithPrelude
                   (fun sb -> sb |> SB.appendFinalTracePrelude |> SB.appendLine)
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

[<RequireQualifiedAccess>]
module ErrorFormatters =

    let General: IErrorFormatter = ChainErrorFormatter.Instance

    let getByFormat (format: string) : IErrorFormatter =
        match format with
        | null | "" -> General
        | "f" -> FinalMessageErrorFormatter.Instance
        | "c" -> ChainMessageErrorFormatter.Instance
        | "F" -> FinalErrorFormatter.Instance
        | "S" -> ChainErrorFormatter.Instance
        | "T" -> TraceErrorFormatter.Instance
        | _ -> raise (FormatException($"The {format} format string is not supported."))
