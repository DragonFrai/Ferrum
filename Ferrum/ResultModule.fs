namespace Ferrum

open System.Diagnostics


type Result<'a> = Result<'a, IError>

[<RequireQualifiedAccess>]
module Result =

    let err (message: string) : Result<'a, IError> =
        Error (Error.err message)

    let context (context: string) (result: Result<'a, IError>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.context context err)

    [<StackTraceHidden>]
    let contextTraced (context: string) (result: Result<'a, IError>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.contextTraced context err)

    let contextWith (context: unit -> string) (result: Result<'a, IError>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.context (context ()) err)

    [<StackTraceHidden>]
    let contextWithTraced (context: unit -> string) (result: Result<'a, IError>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.contextTraced (context ()) err)

    let wrap (result: Result<'a, 'e>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.wrap err)

    [<StackTraceHidden>]
    let wrapTraced (result: Result<'a, 'e>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.wrapTraced err)

    let aggregate (message: string) (result: Result<'a, IError seq>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.aggregate message err)

    [<StackTraceHidden>]
    let aggregateTraced (message: string) (result: Result<'a, IError seq>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.aggregateTraced message err)

    let catchToError (f: unit -> 'a) : Result<'a, IError> =
        try Ok(f ())
        with ex -> Error (Error.ofException ex)

    let getOrRaiseError (result: Result<'a, IError>) : 'a =
        match result with
        | Ok x -> x
        | Error err -> Error.raise err
