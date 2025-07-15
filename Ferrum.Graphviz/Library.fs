namespace Ferrum.Graphviz

open System
open System.Collections.Generic
open System.Text

open Ferrum
open Ferrum.Formatting


[<RequireQualifiedAccess>]
module internal GraphvizUtils =

    let mappedViews (mapper: 'node -> 'a) (children: 'node -> 'node seq) (root: 'node) : 'a seq * ('a * 'a) seq =
        let targets = Queue<'node * 'a>()
        let values = List<'a>()
        let edges = List<'a * 'a>()
        targets.Enqueue((root, mapper root))
        while targets.Count > 0 do
            let node, value = targets.Dequeue()
            values.Add(value)
            for child in children node do
                let childValue = mapper child
                edges.Add((value, childValue))
                targets.Enqueue((child, childValue))
        (values, edges)

    let formatTree (view: 'node -> string) (children: 'node -> 'node seq) (root: 'node) : string =
        let getViewWithCount () =
            let viewsCounts = Dictionary<string, int>()
            fun (node: 'node) ->
                let viewStr = view node
                let usages =
                    let c = viewsCounts.GetValueOrDefault(viewStr, 0) + 1
                    viewsCounts[viewStr] <- c
                    c
                let viewStr = viewStr.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n")
                let viewStr = if usages <= 1 then $"\"{viewStr}\"" else $"\"{viewStr} ({usages})\""
                viewStr
        let sb = StringBuilder()
        let nodes, edges = mappedViews (getViewWithCount ()) children root

        sb.Append("digraph ErrorGraph {").AppendLine() |> ignore
        for node in nodes do
            sb.Append("  ").Append(node).Append(";").AppendLine() |> ignore
        for nodeFrom, nodeTo in edges do
            sb.Append("  ").Append(nodeFrom).Append(" -> ").Append(nodeTo).Append(";").AppendLine() |> ignore
        sb.Append("}").AppendLine() |> ignore

        sb.ToString()


type GraphvizErrorFormatter private () =

    static let _instance = GraphvizErrorFormatter()
    static member Instance: GraphvizErrorFormatter = _instance

    member this.Format(ex: exn) : string =
        let getExceptionSources (ex: exn) : exn seq =
            match ex with
            | :? AggregateException as ex ->
                ex.InnerExceptions
            | ex ->
                match ex.InnerException with
                | null -> Seq.empty
                | ex -> Seq.singleton ex
        ex |> GraphvizUtils.formatTree (fun e -> e.Message) getExceptionSources

    interface IErrorFormatter with
        member this.Format(error) =
            let getErrorSources (err: IError) : IError seq =
                match err with
                | :? IAggregateError as err ->
                    err.InnerErrors
                | err ->
                    match err.InnerError with
                    | null -> Seq.empty
                    | err -> Seq.singleton err
            error |> GraphvizUtils.formatTree (fun e -> e.Message) getErrorSources


