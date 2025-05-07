[<RequireQualifiedAccess>]
module Ferrum.Examples.CustomErrors

open Ferrum


type SimpleError =
    | SimpleCase
    with
        interface IError with
            member this.Message =
                match this with
                | SimpleCase -> "Some simple error case"
            member this.InnerError =
                ValueNone

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
                | Source simpleError -> ValueSome simpleError
                | SomeError -> ValueNone

let run () =

    let simpleError = SimpleError.SimpleCase
    printfn $" > {simpleError.Format(ChainMessageErrorFormatter.Instance)}"
    // > Some simple error case

    let complexErrorSomeError = ComplexError.SomeError
    printfn $" > {complexErrorSomeError.Format(ChainMessageErrorFormatter.Instance)}"
    // > Some complex error case

    let complexErrorWithSource = ComplexError.Source SimpleError.SimpleCase
    printfn $" > {complexErrorWithSource.Format(ChainMessageErrorFormatter.Instance)}"
    // > Error caused by simple error source: Some simple error case

    ()
