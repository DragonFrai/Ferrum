module Ferrum.Tests.GraphvizFormattingTests

open System
open Xunit
open Ferrum
open Ferrum.Graphviz


let singleError = Error.err "Final"
let chainError = Error.context "Final" (Error.context "Middle" <| Error.err "Root")
let aggregateError = Error.aggregate "Agg" (["Err1"; "Err2"] |> List.map Error.err)
let aggregateWihDupsError = Error.aggregate "Err" (["Err"; "Err"] |> List.map Error.err)
let aggregate2Error =
    Error.aggregate "Agg" [
        Error.context "Ctx1" (Error.err "Err1")
        Error.context "Ctx2" (Error.err "Err2")
    ]

let aggregate2Exn () =
    AggregateException("Agg", [
        Exception("Ctx1", Exception("Err1"))
        Exception("Ctx2", Exception("Err2"))
    ])



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

let formatter = GraphvizErrorFormatter.Instance

[<Fact>]
let ``Single node graph`` () =
    let str =
        """digraph ErrorGraph {
  "Final";
}
"""
    do assertFormat formatter singleError str

[<Fact>]
let ``Sequence graph`` () =
    let str =
        """digraph ErrorGraph {
  "Final";
  "Middle";
  "Root";
  "Final" -> "Middle";
  "Middle" -> "Root";
}
"""
    do assertFormat formatter chainError str

[<Fact>]
let ``Tree graph`` () =
    let str =
        """digraph ErrorGraph {
  "Agg";
  "Err1";
  "Err2";
  "Agg" -> "Err1";
  "Agg" -> "Err2";
}
"""
    do assertFormat formatter aggregateError str

[<Fact>]
let ``Tree graph with dups`` () =
    let str =
        """digraph ErrorGraph {
  "Err";
  "Err (2)";
  "Err (3)";
  "Err" -> "Err (2)";
  "Err" -> "Err (3)";
}
"""
    do assertFormat formatter aggregateWihDupsError str

[<Fact>]
let ``Formatting is breadth-first`` () =
    let str =
        """digraph ErrorGraph {
  "Agg";
  "Ctx1";
  "Ctx2";
  "Err1";
  "Err2";
  "Agg" -> "Ctx1";
  "Agg" -> "Ctx2";
  "Ctx1" -> "Err1";
  "Ctx2" -> "Err2";
}
"""
    do assertFormat formatter aggregate2Error str

[<Fact>]
let ``Exception formatting works`` () =
    let ex =
        try raise (aggregate2Exn ())
        with ex -> ex

    let str =
        """digraph ErrorGraph {
  "Agg (Ctx1) (Ctx2)";
  "Ctx1";
  "Ctx2";
  "Err1";
  "Err2";
  "Agg (Ctx1) (Ctx2)" -> "Ctx1";
  "Agg (Ctx1) (Ctx2)" -> "Ctx2";
  "Ctx1" -> "Err1";
  "Ctx2" -> "Err2";
}
"""
    Assert.Equal(str, GraphvizErrorFormatter.Instance.Format(ex))

// [<Fact>]
// let ``Foo`` () =
//     let exs () = Seq.init 10 (fun i -> Exception($"Exn{i}"))
//     let ex = AggregateException("Agg", (Seq.init 10 (fun i -> AggregateException($"Agg{i}", exs ()) :> Exception)))
//     failwith $"{ex.Message}"
//     ()
