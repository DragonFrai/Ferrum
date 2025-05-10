namespace Ferrum

open System.Diagnostics


type Result<'a> = Result<'a, IError>

[<RequireQualifiedAccess>]
module Result =

    let failure (message: string) : Result<'a, IError> =
        Error (Error.failure message)

    [<StackTraceHidden>]
    let failureTraced (message: string) : Result<'a, IError> =
        Error (Error.failureTraced message)

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

    let boxError (result: Result<'a, 'e>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.box err)

    [<StackTraceHidden>]
    let boxErrorTraced (result: Result<'a, 'e>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.boxTraced err)

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
        try Ok (f ())
        with ex -> Error (Error.ofException ex)

    let getOrRaiseError (result: Result<'a, IError>) : 'a =
        match result with
        | Ok x -> x
        | Error err -> Error.raise err
