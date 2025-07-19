[<RequireQualifiedAccess>]
module Ferrum.Examples.ErrorFormatting

open Ferrum
open Ferrum.FSharp
open Ferrum.Formatting.Formatters




let run () =

    let error =
        Error.message "File already exists"
        |> Error.contextTraced "Name already used"
        |> Error.contextTraced "User not created"

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
    // [0] Error: User not created
    // [1] Cause: Name already used
    // [2] Cause: File already exists
    // Trace [1]:
    //    at ...
    //    at ...
    printf $"D >\n{Error.formatD error}"

    // X >
    // [0] Error: User not created
    // [1] Cause: Name already used
    // [2] Cause: File already exists
    // Trace [0]:
    //    at ...
    //    at ...
    // Trace [1]:
    //    at ...
    //    at ...
    printf $"X >\n{Error.formatX error}"
