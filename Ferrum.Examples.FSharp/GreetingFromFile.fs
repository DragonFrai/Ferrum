[<RequireQualifiedAccess>]
module Ferrum.Examples.GreetingFromFile

open Ferrum
open Ferrum.FSharp


[<RequireQualifiedAccess>]
type IOError =
    | FileNotFound

let readFile (_fileName: string) : Result<string, IOError> =
    Error IOError.FileNotFound

let makeGreeting () : Result<string, IError> =
    let readNameResult = readFile "name.txt" |> Result.boxError |> Result.context "Name is unknown"
    match readNameResult with
    | Error err -> Error err
    | Ok name -> Ok $"Hello, {name}"

let greet () : Result<unit, IError> =
    let makeGreetingResult = makeGreeting () |> Result.context "Greeting is not build"
    match makeGreetingResult with
    | Error err -> Error err
    | Ok greeting ->
        printfn $"{greeting}"
        Ok ()

let run () =
    let res: Result<unit, IError> = greet ()
    printfn $" > {Utils.formatResult Error.formatS res}"
    // > Error (Greeting is not build: Name is unknown: FileNotFound)
