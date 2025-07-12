[<RequireQualifiedAccess>]
module Ferrum.Examples.DynamicErrors

open Ferrum
open Ferrum.FSharp


type SomeError = SomeError

let run () =

    // Creating primary message errors
    let messageError: IError = Error.failure "This is some error"
    let messageResult: Result<unit, IError> = Result.failure "This is some error"
    printfn $" > {Error.formatS messageError}"
    printfn $" > {Utils.formatResult Error.formatS messageResult}"
    // > This is some error
    // > Error (This is some error)

    // Adding error context
    let contextualError: IError = Error.context "Final error" (Error.failure "Root error")
    let contextualResult: Result<unit, IError> = Result.context "Top error" (Result.failure "Root error")
    printfn $" > {Error.formatS contextualError}"
    printfn $" > {Utils.formatResult Error.formatS contextualResult}"
    // > Final error: Root error
    // > Error (Top error: Root error)

    // Boxing not IError error types to IError
    let boxedError: IError = Error.box SomeError
    let boxedResult: Result<unit, IError> = Result.boxError (Error SomeError)
    printfn $" > {Error.formatS boxedError}"
    printfn $" > {Utils.formatResult Error.formatS boxedResult}"
    // > SomeError
    // > Error (SomeError)

    ()
