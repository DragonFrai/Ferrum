[<RequireQualifiedAccess>]
module Ferrum.Examples.GeneralWrapping

open Ferrum
open Ferrum.FSharp


type MyError = | MyError

let run () =

    let wrappedError = Error.box MyError
    printfn $"{Error.formatS wrappedError}"
    // MyError

    let contextError = Error.context "Some context" wrappedError
    printfn $"{Error.formatS contextError}"
    // Some context: MyError
