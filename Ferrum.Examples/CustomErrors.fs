[<RequireQualifiedAccess>]
module Ferrum.Examples.CustomErrors

open Ferrum
open Ferrum.Formatting


type SimpleError =
    | SimpleCase
    with
        interface IError with
            member this.Reason =
                match this with
                | SimpleCase -> "Some simple error case"
            member this.Source =
                ValueNone

type ComplexError =
    | Source of SimpleError
    | SomeError
    with
        interface IError with
            member this.Reason =
                match this with
                | Source _ -> "Error caused by simple error source"
                | SomeError -> "Some complex error case"
            member this.Source =
                match this with
                | Source simpleError -> ValueSome simpleError
                | SomeError -> ValueNone

let run () =

    let simpleError = SimpleError.SimpleCase
    printfn $" > {simpleError.Format(ChainErrorFormatter.Instance)}"
    // > Some simple error case

    let complexErrorSomeError = ComplexError.SomeError
    printfn $" > {complexErrorSomeError.Format(ChainErrorFormatter.Instance)}"
    // > Some complex error case

    let complexErrorWithSource = ComplexError.Source SimpleError.SimpleCase
    printfn $" > {complexErrorWithSource.Format(ChainErrorFormatter.Instance)}"
    // > Error caused by simple error source: Some simple error case

    ()
