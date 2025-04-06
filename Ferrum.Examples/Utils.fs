namespace Ferrum.Examples

open Ferrum


[<RequireQualifiedAccess>]
module Utils =

    // let formatResultVerbose (result: Result<'a, IError>) : string =
    //     match result with
    //     | Ok x -> $"Ok (%A{x})"
    //     | Error err -> $"Error ({err.Format(ErrorFormatters.MultilineErrorFormatter.Instance)})"

    let formatResultChain (result: Result<'a, IError>) : string =
        match result with
        | Ok x -> $"Ok (%A{x})"
        | Error err -> $"Error ({err.Format(ChainErrorFormatter.Instance)})"
