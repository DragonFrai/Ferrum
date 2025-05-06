namespace Ferrum

open System.Diagnostics

open Ferrum.Internal


type DynamicError =
    val private reason: string
    val private source: IError voption
    val private sources: IError seq
    val private isAggregate: bool
    val private trace: string
    val private localTrace: StackTrace

    new(reason: string) =
        { reason = reason; source = ValueNone; sources = Seq.empty; isAggregate = false; trace = null; localTrace = null }

    new(reason: string, source: IError) =
        { reason = reason; source = ValueSome source; sources = Seq.singleton source; isAggregate = false; trace = null; localTrace = null }

    new(reason: string, sources: IError seq) =
        { reason = reason; source = Utils.tryFirst sources; sources = sources; isAggregate = true; trace = null; localTrace = null }

    new(reason: string, trace: string) =
        { reason = reason; source = ValueNone; sources = Seq.empty; isAggregate = false; trace = trace; localTrace = null }

    new(reason: string, source: IError, trace: string) =
        { reason = reason; source = ValueSome source; sources = Seq.singleton source; isAggregate = false; trace = trace; localTrace = null }

    new(reason: string, sources: IError seq, trace: string) =
        { reason = reason; source = Utils.tryFirst sources; sources = sources; isAggregate = true; trace = trace; localTrace = null }

    new(reason: string, localTrace: StackTrace) =
        { reason = reason; source = ValueNone; sources = Seq.empty; isAggregate = false; trace = localTrace.ToString(); localTrace = localTrace }

    new(reason: string, source: IError, localTrace: StackTrace) =
        { reason = reason; source = ValueSome source; sources = Seq.singleton source; isAggregate = false; trace = localTrace.ToString(); localTrace = localTrace }

    new(reason: string, sources: IError seq, localTrace: StackTrace) =
        { reason = reason; source = Utils.tryFirst sources; sources = sources; isAggregate = true; trace = localTrace.ToString(); localTrace = localTrace }

    interface IError with
        member this.Reason = this.reason
        member this.Source = this.source

    interface IAggregateError with
        member this.IsAggregate = this.isAggregate
        member this.Sources = this.sources

    interface ITracedError with
        member this.StackTrace = this.trace
        member this.LocalStackTrace = this.localTrace
