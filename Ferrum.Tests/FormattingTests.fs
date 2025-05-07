module Ferrum.Tests.FormattingTests

open System
open Xunit
open Ferrum


let singleError = Error.anyhow "Final"
let chainError = Error.context "Final" <| (Error.context "Middle" <| Error.anyhow "Root")
let tracedSingleError: IError = DynamicError("Final", "  at final\n")
let tracedChainError: IError =
    DynamicError(
        "Final",
        DynamicError(
            "Middle",
            DynamicError(
                "Root",
                "  at root\n"
            ),
            "  at middle\n"
        ),
        "  at final\n"
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
let ``FinalErrorFormatter works`` () =
    let fmt = FinalMessageErrorFormatter.Instance
    do assertFormat fmt singleError "Final"
    do assertFormat fmt chainError "Final"
    do assertFormat fmt tracedSingleError "Final"
    do assertFormat fmt tracedChainError "Final"

[<Fact>]
let ``FinalMultilineErrorFormatter works`` () =
    let fmt = FinalErrorFormatter.Instance
    do assertFormat fmt singleError "Error: Final\n"
    do assertFormat fmt chainError "Error: Final\n"
    do assertFormat fmt tracedSingleError "Error: Final\n  at final\n"
    do assertFormat fmt tracedChainError "Error: Final\n  at final\n"

[<Fact>]
let ``ChainErrorFormatter works`` () =
    let fmt = ChainMessageErrorFormatter.Instance
    do assertFormat fmt singleError "Final"
    do assertFormat fmt chainError "Final: Middle: Root"
    do assertFormat fmt tracedSingleError "Final"
    do assertFormat fmt tracedChainError "Final: Middle: Root"

[<Fact>]
let ``ChainMultilineErrorFormatter works`` () =
    let fmt = TraceErrorFormatter.Instance
    do assertFormat fmt singleError "Error: Final\n"
    do assertFormat fmt chainError "Error: Final\nCaused by: Middle\nCaused by: Root\n"
    do assertFormat fmt tracedSingleError "Error: Final\n  at final\n"
    do assertFormat fmt tracedChainError "Error: Final\n  at final\nCaused by: Middle\n  at middle\nCaused by: Root\n  at root\n"

[<Fact>]
let ``ChainShortErrorFormatter works`` () =
    let fmt = ShortErrorFormatter.Instance
    do assertFormat fmt singleError "Error: Final\n"
    do assertFormat fmt chainError "Error: Final\nCaused by: Middle\nCaused by: Root\n"
    do assertFormat fmt tracedSingleError "Error: Final\nFinal stack trace:\n  at final\n"
    do assertFormat fmt tracedChainError "Error: Final\nCaused by: Middle\nCaused by: Root\nFinal stack trace:\n  at final\n"

