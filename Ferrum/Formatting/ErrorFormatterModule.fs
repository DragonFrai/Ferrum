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
        | "m" | "M" | "1" | "l1" | "L1" -> MessageErrorFormatter.Instance
        | "s" | "S" | "2" | "l2" | "L2" -> SummaryErrorFormatter.Instance
        | "d" | "D" | "3" | "l3" | "L3" -> DetailedErrorFormatter.Instance
        | "x" | "X" | "4" | "l4" | "L4" -> DiagnosticErrorFormatter.Instance
        | _ -> raise (FormatException($"The {format} format string is not supported."))
