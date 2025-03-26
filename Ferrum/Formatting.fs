namespace Ferrum.Formatting

open System.Text
open Ferrum


type IErrorFormatter =
    abstract Format: error: IError -> string

module ErrorFormatters =

    type TopErrorFormatter private () =
        static member Instance: TopErrorFormatter = TopErrorFormatter()
        interface IErrorFormatter with
            member this.Format(error) =
                error.Reason


    type ChainErrorFormatter private () =
        static member Instance: ChainErrorFormatter = ChainErrorFormatter()
        static member private Delimiter = ": "
        interface IErrorFormatter with
            member this.Format(error) =
                let rec loop (error: IError) (sb: StringBuilder) =
                    match error.Source with
                    | ValueNone -> sb.Append(error.Reason).ToString()
                    | ValueSome source -> loop source (sb.Append(error.Reason).Append(ChainErrorFormatter.Delimiter))
                let expectedLength =
                    error.Chain()
                    |> Seq.map (fun err -> err.Reason.Length + ChainErrorFormatter.Delimiter.Length)
                    |> Seq.sum
                    |> fun x -> (max (x - 2) 0)
                loop error (StringBuilder(expectedLength))


    type MultilineErrorFormatter private () =
        static member Instance: MultilineErrorFormatter = MultilineErrorFormatter()
        interface IErrorFormatter with
            member this.Format(error) =
                let rec loop (error: IError) (depth: int) (sb: StringBuilder) =
                    let sb =
                        match depth with
                        | 0 -> sb.Append("Error: ").Append(error.Reason).AppendLine()
                        | _ -> sb.AppendLine().Append("Caused by: ").Append(error.Reason).AppendLine()
                    match error.Source with
                    | ValueNone -> sb.ToString()
                    | ValueSome source -> loop source (depth + 1) sb
                let expectedLength =
                    error.Chain()
                    |> Seq.map (fun err -> err.Reason.Length + 13)
                    |> Seq.sum
                    |> fun x -> x + 16
                loop error 0 (StringBuilder(expectedLength))


    type MultilineTraceErrorFormatter private () =
        static member Instance: MultilineTraceErrorFormatter = MultilineTraceErrorFormatter()
        interface IErrorFormatter with
            member this.Format(error) =
                let appendTraceOrLine (error: IError) (sb: StringBuilder) : StringBuilder =
                    match error.StackTrace with
                    | ValueNone -> sb.AppendLine()
                    | ValueSome stackTrace -> sb.AppendLine().Append("Trace: ").AppendLine().Append(stackTrace)
                let rec loop (error: IError) (depth: int) (sb: StringBuilder) =
                    let sb =
                        match depth with
                        | 0 -> sb.Append("Error: ").Append(error.Reason) |> appendTraceOrLine error
                        | _ -> sb.AppendLine().Append("Caused by: ").Append(error.Reason) |> appendTraceOrLine error
                    match error.Source with
                    | ValueNone -> sb.ToString()
                    | ValueSome source -> loop source (depth + 1) sb
                let minimalLength =
                    error.Chain()
                    |> Seq.map (fun err -> err.Reason.Length + 13)
                    |> Seq.sum
                    |> fun x -> x + 16
                loop error 0 (StringBuilder(minimalLength))


[<AutoOpen>]
module FormattingExtensions =

    type IError with

        member inline this.Format(formatter: IErrorFormatter) : string =
            formatter.Format(this)

        member inline this.FormatTop() : string =
            (ErrorFormatters.TopErrorFormatter.Instance :> IErrorFormatter).Format(this)

        member inline this.FormatChain() : string =
            (ErrorFormatters.ChainErrorFormatter.Instance :> IErrorFormatter).Format(this)

        member inline this.FormatMultiline() : string =
            (ErrorFormatters.MultilineErrorFormatter.Instance :> IErrorFormatter).Format(this)

        member inline this.FormatMultilineTrace() : string =
            (ErrorFormatters.MultilineTraceErrorFormatter.Instance :> IErrorFormatter).Format(this)


    [<RequireQualifiedAccess>]
    module Error =

        let inline format (formatter: IErrorFormatter) (err: IError) : string =
            formatter.Format(err)

        let inline formatTop (err: IError) : string =
            (ErrorFormatters.TopErrorFormatter.Instance :> IErrorFormatter).Format(err)

        let inline formatChain (err: IError) : string =
            (ErrorFormatters.ChainErrorFormatter.Instance :> IErrorFormatter).Format(err)

        let inline formatMultiline (err: IError) : string =
            (ErrorFormatters.MultilineErrorFormatter.Instance :> IErrorFormatter).Format(err)

        let inline formatMultilineTrace (err: IError) : string =
            (ErrorFormatters.MultilineTraceErrorFormatter.Instance :> IErrorFormatter).Format(err)
