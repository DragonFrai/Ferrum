[<RequireQualifiedAccess>]
module Ferrum.Examples.GreetingFromFile

open Ferrum


[<RequireQualifiedAccess>]
type IOError =
    | FileNotFound

let readFile (_fileName: string) : Result<string, IOError> =
    Error IOError.FileNotFound

let makeGreeting (readFile: string -> Result<string, IOError>) : Result<string, IError> =
    let readNameResult = readFile "name.txt" |> Result.wrap |> Result.context "Name is unknown"
    match readNameResult with
    | Error err -> Error err
    | Ok name -> Ok $"Hello, {name}"

let greet () : Result<unit, IError> =
    let makeGreetingResult = makeGreeting readFile |> Result.context "Greeting is not build"
    match makeGreetingResult with
    | Error err -> Error err
    | Ok greeting ->
        printfn $"{greeting}"
        Ok ()

let run () =
    let res: Result<unit, IError> = greet ()
    printfn $" > {Utils.formatResultChain res}"
    // > Error (Greeting is not build: Name is unknown: FileNotFound)
