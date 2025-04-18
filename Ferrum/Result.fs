namespace Ferrum


type Result<'a> = Result<'a, IError>

[<RequireQualifiedAccess>]
module Result =

    let message (message: string) : Result<'a, IError> =
        Error (Error.message message)

    let context (context: string) (result: Result<'a, IError>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.context context err)

    let wrap (result: Result<'a, 'e>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.wrap err)

    let aggregate (reason: string) (result: Result<'a, IError seq>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.aggregate reason err)

    let catchToError (f: unit -> 'a) : Result<'a, IError> =
        try Ok(f ())
        with ex -> Error (Error.ofException ex)

    let getOrThrowError (result: Result<'a, IError>) : 'a =
        match result with
        | Ok x -> x
        | Error err -> Error.throw err
