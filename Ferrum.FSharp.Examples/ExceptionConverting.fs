[<RequireQualifiedAccess>]
module Ferrum.Examples.ExceptionConverting

open Ferrum
open Ferrum.FSharp


let run () =

    let ex = exn("Some exn", exn("Inner exn"))
    let err = Error.ofException ex
    printfn $"{Error.formatS err}"
    // Some exn: Inner exn

    let err = Error.context "Some error" (Error.message "Inner error")
    let ex = Error.toException err
    printfn $"{ex}"
    // Ferrum.ErrorException: Some error
    //  ---> Ferrum.ErrorException: Inner error
    //    --- End of inner exception stack trace ---

