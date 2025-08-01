namespace Ferrum.FSharp

open System.Diagnostics
open FSharp.Core
open Ferrum


[<RequireQualifiedAccess>]
module Result =

    let message (message: string) : Result<'a, IError> =
        Error (Error.message message)

    [<StackTraceHidden>]
    let messageTraced (message: string) : Result<'a, IError> =
        Error (Error.messageTraced message)

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

    let aggregate (message: string) (result: Result<'a, IError seq>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.aggregate message err)

    [<StackTraceHidden>]
    let aggregateTraced (message: string) (result: Result<'a, IError seq>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.aggregateTraced message err)

    let ofExnResult (result: Result<'a, exn>) : Result<'a, IError> =
        Result.mapError Error.ofException result

    let toExnResult (result: Result<'a, IError>) : Result<'a, exn> =
        Result.mapError Error.toException result

    let boxError (result: Result<'a, 'e>) : Result<'a, IError> =
        match result with
        | Ok x -> Ok x
        | Error err -> Error (Error.box err)

    let catchToError (f: unit -> 'a) : Result<'a, IError> =
        try Ok (f ())
        with ex -> Error (Error.ofException ex)

    let getOrRaiseError (result: Result<'a, IError>) : 'a =
        match result with
        | Ok x -> x
        | Error err -> raise (Error.toException err)
