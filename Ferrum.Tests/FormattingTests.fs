module Ferrum.Tests.FormattingTests

open System
open Ferrum.Formatting
open Xunit
open Ferrum


let singleError = Error.message "Final"
let chainError = Error.context "Final" <| (Error.context "Middle" <| Error.message "Root")

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
    let fmt = ErrorFormatters.FinalErrorFormatter.Instance
    do assertFormat fmt singleError "Final"
    do assertFormat fmt chainError "Final"

[<Fact>]
let ``ChainErrorFormatter works`` () =
    let fmt = ErrorFormatters.ChainErrorFormatter.Instance
    do assertFormat fmt singleError "Final"
    do assertFormat fmt chainError "Final: Middle: Root"

[<Fact>]
let ``MultilineErrorFormatter works`` () =
    let fmt = ErrorFormatters.MultilineErrorFormatter.Instance
    do assertFormat fmt singleError "Error: Final\n"
    do assertFormat fmt chainError "Error: Final\n\nCaused by: Middle\n\nCaused by: Root\n"

[<Fact>]
let ``MultilineTraceErrorFormatter works`` () =
    let fmt = ErrorFormatters.MultilineErrorFormatter.Instance
    do assertFormat fmt singleError "Error: Final\n"
    do assertFormat fmt chainError "Error: Final\n\nCaused by: Middle\n\nCaused by: Root\n"

