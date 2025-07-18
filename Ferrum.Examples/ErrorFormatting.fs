[<RequireQualifiedAccess>]
module Ferrum.Examples.ErrorFormatting

open Ferrum
open Ferrum.FSharp
open Ferrum.Formatting.Formatters




let run () =

    let error =
        Error.context "User not created" (Error.context "Name already used" (Error.message "File already exists"))
    let errorTraced =
        Error.contextTraced "User not created"
            (Error.contextTraced "Name already used"
                (Error.messageTraced "File already exists"))

    // Using formatter
    printfn $"{Error.format SummaryErrorFormatter.Instance error}"

    // Using literal format
    printfn $"""{(Error.formatBy "s" error)}"""

    // Using literal format functions
    printfn $"{Error.formatS error}"

    // Using interpolated literal format
    printfn $"{error:s}"

    // Using default formater ToString
    printfn $"{error:s}"

    // === Printing using different formatters ===

    // M > User not created
    printfn $"M > {Error.formatM error}"

    // S > User not created: Name already used: File already exists
    printfn $"S > {Error.formatS error}"

    // D >
    // Error: User not created
    // Cause: Name already used
    // Cause: File not found
    //
    printfn $"D >\n{Error.formatD error}"

    // X >
    // Error: User not created
    //    at ...
    // Cause: Name already used
    //    at ...
    // Cause: File not found
    //    at ...
    //
    printfn $"X >\n{Error.formatD errorTraced}"
