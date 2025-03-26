[<RequireQualifiedAccess>]
module Ferrum.Examples.DynamicErrors

open Ferrum
open Ferrum.Formatting


type SomeError = SomeError

let run () =

    let messageError: IError = Error.message "This is some error"
    printfn $" > {messageError.FormatChain()}"
    // > This is some error

    let contextualError: IError = Error.context "Top error" (Error.message "Root error")
    printfn $" > {contextualError.FormatChain()}"
    // > Top error: Root error

    let wrappedError: IError = Error.wrap SomeError
    printfn $" > {wrappedError.FormatChain()}"
    // > SomeError

    let messageResult: Result<unit, IError> = Result.message "This is some error"
    printfn $" > {Utils.formatResultChain messageResult}"
    // > Error (This is some error)

    let contextualResult: Result<unit, IError> = Result.context "Top error" (Result.message "Root error")
    printfn $" > {Utils.formatResultChain contextualResult}"
    // > Error (Top error: Root error)

    let wrappedResult: Result<unit, IError> = Result.wrap (Error SomeError)
    printfn $" > {Utils.formatResultChain wrappedResult}"
    // > Error (SomeError)

    ()
