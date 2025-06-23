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
    let [<Literal>] Cause = "Cause: "

    let inline append (str: string) (sb: StringBuilder) : StringBuilder =
        sb.Append(str)

    let inline appendLine (sb: StringBuilder) : StringBuilder =
        sb.AppendLine()

    let inline toString (sb: StringBuilder) : string =
        sb.ToString()

    let inline appendMessage (error: IError) (sb: StringBuilder) : StringBuilder =
        sb.Append(String.notEmptyOr NoMessage error.Message)

    let inline appendTrace (error: IError) (sb: StringBuilder) : StringBuilder =
        let trace = Utils.stackTraceChecked error
        if String.IsNullOrEmpty(trace) then
            sb
        else
            let sb =
                if trace.StartsWith("   at")
                then sb
                else sb.Append("   at ")
            let sb =
                sb.Append(trace)
            let sb =
                if trace.EndsWith(Environment.NewLine) || trace.EndsWith('\n') || trace.EndsWith('\r') || trace.EndsWith("\r\n")
                then sb
                else sb.AppendLine()
            sb

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
        while innerError <> null do
            let innerErrorValue = innerError
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

type MessageErrorFormatter private () =
    static let _instance = MessageErrorFormatter()
    static member Instance: MessageErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            String.notEmptyOr SB.NoMessage error.Message

type SummaryErrorFormatter private () =
    static let _instance = SummaryErrorFormatter()
    static member Instance: SummaryErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            StringBuilder()
            |> SB.appendChain
                (fun error sb -> sb |> SB.appendMessage error)
                (fun error sb -> sb |> SB.append SB.Colon |> SB.appendMessage error)
                error
            |> SB.toString

type DetailedErrorFormatter private () =
    static let _instance = DetailedErrorFormatter()
    static member Instance: DetailedErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            StringBuilder()
            |> SB.appendChainWith
                (fun error sb ->
                    sb
                    |> SB.append SB.Error
                    |> SB.appendMessage error
                    |> SB.appendLine)
                (fun error sb ->
                    sb
                    |> SB.append SB.Cause
                    |> SB.appendMessage error
                    |> SB.appendLine)
                (fun error sb ->
                    sb |> SB.appendTrace error)
                error
            |> SB.toString

type DiagnosticErrorFormatter private () =
    static let _instance = DiagnosticErrorFormatter()
    static member Instance: DiagnosticErrorFormatter = _instance
    interface IErrorFormatter with
        member this.Format(error) =
            StringBuilder()
            |> SB.appendChain
                (fun error sb ->
                    sb
                    |> SB.append SB.Error
                    |> SB.appendMessage error
                    |> SB.appendLine
                    |> SB.appendTrace error)
                (fun error sb ->
                    sb
                    |> SB.append SB.Cause
                    |> SB.appendMessage error
                    |> SB.appendLine
                    |> SB.appendTrace error)
                error
            |> SB.toString
