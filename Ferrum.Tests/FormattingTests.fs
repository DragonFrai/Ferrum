module Ferrum.Tests.FormattingTests

open System
open Ferrum.Formatting
open Xunit
open Ferrum
open Ferrum.Errors
open Ferrum.Formatting.Formatters
open Ferrum.FSharp


let singleError = Error.message "Final"
let chainError = Error.context "Final" <| (Error.context "Middle" <| Error.message "Root")
let tracedSingleError: IError = DynamicError("Final", "   at final\n")
let tracedChainError: IError =
    DynamicError(
        "Final",
        DynamicError(
            "Middle",
            DynamicError(
                "Root",
                "   at root\n"
            ),
            "   at middle\n"
        ),
        "   at final\n"
    )
let tracedChainErrorMiddleTraceOnly: IError =
    DynamicError(
        "Final",
        DynamicError(
            "Middle",
            DynamicError(
                "Root"
            ),
            "   at middle\n"
        )
    )


let stringOfLines (lines: string seq) : string =
    String.Join("\n", lines)

let stringOfLinesTrailing (lines: string seq) : string =
    String.Join("\n", lines) + "\n"

let assertFormat (formatter: IErrorFormatter) (error: IError) (expected: string) : unit =
    Assert.Equal(expected, formatter.Format(error))

[<Fact>]
let ``MessageErrorFormatter works`` () =
    let fmt = MessageErrorFormatter.Instance
    do assertFormat fmt singleError "Final"
    do assertFormat fmt chainError "Final"
    do assertFormat fmt tracedSingleError "Final"
    do assertFormat fmt tracedChainError "Final"

[<Fact>]
let ``SummaryErrorFormatter works`` () =
    let fmt = SummaryErrorFormatter.Instance
    do assertFormat fmt singleError "Final"
    do assertFormat fmt chainError "Final: Middle: Root"
    do assertFormat fmt tracedSingleError "Final"
    do assertFormat fmt tracedChainError "Final: Middle: Root"

[<Fact>]
let ``DetailedErrorFormatter works`` () =
    let fmt = DetailedErrorFormatter.Instance

    do assertFormat fmt singleError (stringOfLinesTrailing [
        "[0] Error: Final"
    ])

    do assertFormat fmt chainError (stringOfLinesTrailing [
        "[0] Error: Final"
        "[1] Cause: Middle"
        "[2] Cause: Root"
    ])

    do assertFormat fmt tracedSingleError (stringOfLinesTrailing [
        "[0] Error: Final"
        "Trace [0]:"
        "   at final"
    ])

    do assertFormat fmt tracedChainError (stringOfLinesTrailing [
        "[0] Error: Final"
        "[1] Cause: Middle"
        "[2] Cause: Root"
        "Trace [2]:"
        "   at root"
    ])

    do assertFormat fmt tracedChainErrorMiddleTraceOnly (stringOfLinesTrailing [
        "[0] Error: Final"
        "[1] Cause: Middle"
        "[2] Cause: Root"
        "Trace [1]:"
        "   at middle"
    ])


[<Fact>]
let ``DiagnosticErrorFormatter works`` () =
    let fmt = DiagnosticErrorFormatter.Instance

    do assertFormat fmt singleError (stringOfLinesTrailing [
        "[0] Error: Final"
    ])

    do assertFormat fmt chainError (stringOfLinesTrailing [
        "[0] Error: Final"
        "[1] Cause: Middle"
        "[2] Cause: Root"
    ])

    do assertFormat fmt tracedSingleError (stringOfLinesTrailing [
        "[0] Error: Final"
        "Trace [0]:"
        "   at final"
    ])

    do assertFormat fmt tracedChainError (stringOfLinesTrailing [
        "[0] Error: Final"
        "[1] Cause: Middle"
        "[2] Cause: Root"
        "Trace [0]:"
        "   at final"
        "Trace [1]:"
        "   at middle"
        "Trace [2]:"
        "   at root"
    ])

    do assertFormat fmt tracedChainErrorMiddleTraceOnly (stringOfLinesTrailing [
        "[0] Error: Final"
        "[1] Cause: Middle"
        "[2] Cause: Root"
        "Trace [1]:"
        "   at middle"
    ])
