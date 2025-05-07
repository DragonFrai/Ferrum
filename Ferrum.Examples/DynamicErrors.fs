[<RequireQualifiedAccess>]
module Ferrum.Examples.DynamicErrors

open Ferrum


type SomeError = SomeError

let run () =

    let messageError: IError = Error.anyhow "This is some error"
    printfn $" > {Error.format ChainMessageErrorFormatter.Instance messageError}"
    // > This is some error

    let contextualError: IError = Error.context "Top error" (Error.anyhow "Root error")
    printfn $" > {Error.format ChainMessageErrorFormatter.Instance contextualError}"
    // > Top error: Root error

    let wrappedError: IError = Error.wrap SomeError
    printfn $" > {Error.format ChainMessageErrorFormatter.Instance wrappedError}"
    // > SomeError

    let messageResult: Result<unit, IError> = Result.anyhow "This is some error"
    printfn $" > {Utils.formatResultChain messageResult}"
    // > Error (This is some error)

    let contextualResult: Result<unit, IError> = Result.context "Top error" (Result.anyhow "Root error")
    printfn $" > {Utils.formatResultChain contextualResult}"
    // > Error (Top error: Root error)

    let wrappedResult: Result<unit, IError> = Result.wrap (Error SomeError)
    printfn $" > {Utils.formatResultChain wrappedResult}"
    // > Error (SomeError)

    ()
