namespace Ferrum

open System.Text
open Ferrum
open Ferrum.Internal


[<RequireQualifiedAccess>]
module private SB =

    let inline append (str: string) (sb: StringBuilder) : StringBuilder =
        sb.Append(str)

    let inline appendInt (num: int) (sb: StringBuilder) : StringBuilder =
        sb.Append(num)

    let inline appendLine (sb: StringBuilder) : StringBuilder =
        sb.AppendLine()


[<RequireQualifiedAccess>]
module private Utils =

    let [<Literal>] NoReason = "<noreason>"

    let inline notEmptyOrNoReason (reason: string) : string =
        match reason with
        | null | "" -> NoReason
        | reason -> reason

    let reasonLength (error: IError) : int =
        (notEmptyOrNoReason error.Reason).Length

    let appendReason (error: IError) (sb: StringBuilder) : StringBuilder =
        sb.Append(notEmptyOrNoReason error.Reason)

    let inline foldErrorChain<'s>
            ([<InlineIfLambda>] finalErrorHook: 's -> IError -> 's)
            ([<InlineIfLambda>] chainErrorHook: 's -> IError -> 's)
            (state: 's)
            (error: IError)
            : 's =
        let mutable state = state
        do state <- finalErrorHook state error
        do
            let mutable source = error.Source
            while source.IsSome do
                let sourceError = source.Value
                state <- chainErrorHook state sourceError
                source <- sourceError.Source
        state


type FinalErrorFormatter private () =
    static member Instance: FinalErrorFormatter = FinalErrorFormatter()
    interface IErrorFormatter with
        member this.Format(error) =
            Utils.notEmptyOrNoReason error.Reason

type ChainErrorFormatter private () =
    static member Instance: ChainErrorFormatter = ChainErrorFormatter()
    static member private Delimiter = ": "
    interface IErrorFormatter with
        member this.Format(error) =
            let expectedLength =
                Utils.foldErrorChain<int>
                    (fun c error -> c + Utils.reasonLength error)
                    (fun c error -> c + Utils.reasonLength error + ChainErrorFormatter.Delimiter.Length)
                    0
                    error
            let sb =
                Utils.foldErrorChain<StringBuilder>
                    (fun sb error ->
                        sb
                        |> Utils.appendReason error)
                    (fun sb error ->
                        sb
                        |> SB.append ChainErrorFormatter.Delimiter
                        |> Utils.appendReason error)
                    (StringBuilder(expectedLength))
                    error
            sb.ToString()


type MultilineErrorFormatter private () =
    static member Instance: MultilineErrorFormatter = MultilineErrorFormatter()
    static member private FinalPrefix: string = "Error: "
    static member private ChainPrefix: string = "Caused by: "
    interface IErrorFormatter with
        member this.Format(error) =
            let expectedLength =
                Utils.foldErrorChain<int>
                    (fun c error -> c + MultilineErrorFormatter.FinalPrefix.Length + Utils.reasonLength error + 1)
                    (fun c error -> c + MultilineErrorFormatter.ChainPrefix.Length + Utils.reasonLength error + 2)
                    0
                    error
            let sb =
                Utils.foldErrorChain<StringBuilder>
                    (fun sb error ->
                        sb
                        |> SB.append MultilineErrorFormatter.FinalPrefix
                        |> Utils.appendReason error
                        |> SB.appendLine)
                    (fun sb error ->
                        sb
                        |> SB.appendLine
                        |> SB.append MultilineErrorFormatter.ChainPrefix
                        |> Utils.appendReason error
                        |> SB.appendLine)
                    (StringBuilder(expectedLength))
                    error
            sb.ToString()

type MultilineTraceErrorFormatter private () =
    static member Instance: MultilineTraceErrorFormatter = MultilineTraceErrorFormatter()
    interface IErrorFormatter with
        member this.Format(error) =
            let rec loop (topError: IError) (error: IError) (depth: int) (sb: StringBuilder) =
                let sb =
                    match depth with
                    | 0 ->
                        sb
                        |> SB.append "Error: "
                        |> Utils.appendReason error
                        |> SB.appendLine
                    | _ ->
                        sb
                        |> SB.appendLine
                        |> SB.append "Caused by: "
                        |> Utils.appendReason error
                        |> SB.appendLine
                match error.Source with
                | ValueNone ->
                    let sb =
                        match Utils.stackTraceChecked error with
                        | null -> sb
                        | stackTrace -> sb.AppendLine().Append("Trace: ").AppendLine().Append(stackTrace)
                    sb.ToString()
                | ValueSome source -> loop topError source (depth + 1) sb
            let minimalLength = 1024
            loop error error 0 (StringBuilder(minimalLength))

type MultilineTraceAllErrorFormatter private () =
    static member Instance: MultilineTraceAllErrorFormatter = MultilineTraceAllErrorFormatter()
    interface IErrorFormatter with
        member this.Format(error) =
            let appendTraceOrLine (error: IError) (sb: StringBuilder) : StringBuilder =
                match Utils.stackTraceChecked error with
                | null -> sb.AppendLine()
                | stackTrace -> sb.AppendLine().Append("Trace: ").AppendLine().Append(stackTrace)
            let rec loop (error: IError) (depth: int) (sb: StringBuilder) =
                let sb =
                    match depth with
                    | 0 ->
                        sb
                        |> SB.append "Error: "
                        |> Utils.appendReason error
                        |> appendTraceOrLine error
                    | _ ->
                        sb
                        |> SB.appendLine
                        |> SB.append "Caused by: "
                        |> Utils.appendReason error
                        |> appendTraceOrLine error
                match error.Source with
                | ValueNone -> sb.ToString()
                | ValueSome source -> loop source (depth + 1) sb
            let minimalLength = 1024
            loop error 0 (StringBuilder(minimalLength))
