namespace Ferrum

open System


[<RequireQualifiedAccess>]
module ErrorFormatter =

    let inline format (error: IError) (formatter: IErrorFormatter) : string =
        formatter.Format(error)

    let Default: IErrorFormatter = SummaryErrorFormatter.Instance

    let byFormat (format: string) : IErrorFormatter =
        match format with
        | format when String.IsNullOrEmpty(format) -> Default
        | "m" | "M" | "l1" | "L1" -> MessageErrorFormatter.Instance
        | "s" | "S" | "l2" | "L2" -> SummaryErrorFormatter.Instance
        | "d" | "D" | "l3" | "L3" -> DetailedErrorFormatter.Instance
        | "x" | "X" | "l4" | "L4" -> DiagnosticErrorFormatter.Instance
        | _ -> raise (FormatException($"The {format} format string is not supported."))

    let byLevel (level: int) : IErrorFormatter =
        match level with
        | 1 -> MessageErrorFormatter.Instance
        | 2 -> SummaryErrorFormatter.Instance
        | 3 -> DetailedErrorFormatter.Instance
        | 4 -> DiagnosticErrorFormatter.Instance
        | _ -> raise (FormatException($"The {level} format level is not supported."))
