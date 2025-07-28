module Ferrum.Examples.Readme

open System.IO
open Ferrum
open Ferrum.FSharp

module ReadmeExample =

    let readFile (path: string) : Result<string, IError> =
        try
            Ok (File.ReadAllText(path))
        with ex ->
            Error (Error.ofException ex)

    let createHello () : Result<string, IError> =
        readFile "name.txt"
        |> Result.context "Hello not created"
        |> Result.map (fun name -> $"Hello, {name}!")

    let itIsMain (args: string array) : int =
        match createHello () with
        | Error error ->
            // Print:
            // I can't say hello to you: Hello not created: Could not find file '.../name.txt'.
            printfn $"I can't say hello: {Error.formatS error}"
        | Ok helloString ->
            // Print:
            // I say you: Hello, <name>!
            printfn $"I say: {helloString}"
        0
