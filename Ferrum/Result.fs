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
