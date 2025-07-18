module Ferrum.Tests.FormattingTests

open System
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

let stringOfLines (lines: string seq) : string =
    String.Join("\n", lines)

let assertFormat (formatter: IErrorFormatter) (error: IError) (expected: string) : unit =
    Assert.Equal(expected, formatter.Format(error))

let assertFormatByLines (formatter: IErrorFormatter) (error: IError) (testers: (string -> unit) list) : unit =
    let str = formatter.Format(error)
    let lines = str.Split('\n')
    Assert.Equal(testers.Length, lines.Length)
    for line, tester in Seq.zip lines testers do
        do tester line


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
    do assertFormat fmt singleError "Error: Final\n"
    do assertFormat fmt chainError "Error: Final\nCause: Middle\nCause: Root\n"
    do assertFormat fmt tracedSingleError "Error: Final\n   at final\n"
    do assertFormat fmt tracedChainError "Error: Final\nCause: Middle\nCause: Root\n   at root\n"

[<Fact>]
let ``DiagnosticErrorFormatter works`` () =
    let fmt = DiagnosticErrorFormatter.Instance
    do assertFormat fmt singleError "Error: Final\n"
    do assertFormat fmt chainError "Error: Final\nCause: Middle\nCause: Root\n"
    do assertFormat fmt tracedSingleError "Error: Final\n   at final\n"
    do assertFormat fmt tracedChainError "Error: Final\n   at final\nCause: Middle\n   at middle\nCause: Root\n   at root\n"
