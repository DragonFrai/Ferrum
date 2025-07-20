[<RequireQualifiedAccess>]
module Ferrum.Examples.CustomErrors

open Ferrum
open Ferrum.FSharp


type SimpleError =
    | SimpleCase
    with
        interface IError with
            member this.Message =
                match this with
                | SimpleCase -> "Some simple error case"
            member this.InnerError =
                null

type ComplexError =
    | Source of SimpleError
    | SomeError
    with
        interface IError with
            member this.Message =
                match this with
                | Source _ -> "Error caused by simple error source"
                | SomeError -> "Some complex error case"
            member this.InnerError =
                match this with
                | Source simpleError -> simpleError
                | SomeError -> null

let run () =

    let simpleError = SimpleError.SimpleCase
    printfn $" > {simpleError |> Error.formatS}"
    // > Some simple error case

    let complexErrorSomeError = ComplexError.SomeError
    printfn $" > {complexErrorSomeError |> Error.formatS}"
    // > Some complex error case

    let complexErrorWithSource = ComplexError.Source SimpleError.SimpleCase
    printfn $" > {complexErrorWithSource |> Error.formatS}"
    // > Error caused by simple error source: Some simple error case

    ()
