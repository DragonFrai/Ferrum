namespace Ferrum

open System


[<RequireQualifiedAccess>]
module ErrorFormatter =

    let inline format (error: IError) (formatter: IErrorFormatter) : string =
        formatter.Format(error)

    let Default: IErrorFormatter = ChainErrorFormatter.Instance

    let byFormat (format: string) : IErrorFormatter =
        match format with
        | null | "" -> Default
        | "f" -> FinalMessageErrorFormatter.Instance
        | "c" -> ChainMessageErrorFormatter.Instance
        | "F" -> FinalErrorFormatter.Instance
        | "C" -> ChainErrorFormatter.Instance
        | "T" -> TraceErrorFormatter.Instance
        | _ -> raise (FormatException($"The {format} format string is not supported."))
