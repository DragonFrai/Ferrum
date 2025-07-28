[<RequireQualifiedAccess>]
module Ferrum.Examples.GeneralWrapping

open Ferrum
open Ferrum.FSharp


type MyError = | MyError

let run () =

    let boxedError = Error.box MyError
    printfn $"{Error.formatS boxedError}"
    // MyError

    let contextError = Error.context "Some context" boxedError
    printfn $"{Error.formatS contextError}"
    // Some context: MyError
