[<RequireQualifiedAccess>]
module Ferrum.Examples.ErrorFormatting

open Ferrum




let run () =

    let error = Error.context "User not created" (Error.context "Name already used" (Error.err "File already exists"))
    let errorTraced =
        Error.contextTraced "User not created"
            (Error.contextTraced "Name already used"
                (Error.errTraced "File already exists"))

    //

    printfn " > message formatting <"
    // printfn $"{error:f}" // User not created
    // printfn $"{error:c}" // User not created: Name already used



    // printfn $"{error:C}"
    printfn $"{errorTraced:F}"
    printfn $"{errorTraced:C}"
    printfn $"{errorTraced:T}"
