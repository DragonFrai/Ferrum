namespace Ferrum

open System.Diagnostics

open Ferrum.Internal


type DynamicError =
    val private message: string
    val private innerError: IError voption
    val private innerErrors: IError seq
    val private isAggregate: bool
    val private trace: string
    val private localTrace: StackTrace

    new(message: string) =
        { message = message; innerError = ValueNone; innerErrors = Seq.empty; isAggregate = false; trace = null; localTrace = null }

    new(message: string, source: IError) =
        { message = message; innerError = ValueSome source; innerErrors = Seq.singleton source; isAggregate = false; trace = null; localTrace = null }

    new(message: string, sources: IError seq) =
        { message = message; innerError = Utils.tryFirst sources; innerErrors = sources; isAggregate = true; trace = null; localTrace = null }

    new(message: string, trace: string) =
        { message = message; innerError = ValueNone; innerErrors = Seq.empty; isAggregate = false; trace = trace; localTrace = null }

    new(message: string, source: IError, trace: string) =
        { message = message; innerError = ValueSome source; innerErrors = Seq.singleton source; isAggregate = false; trace = trace; localTrace = null }

    new(message: string, sources: IError seq, trace: string) =
        { message = message; innerError = Utils.tryFirst sources; innerErrors = sources; isAggregate = true; trace = trace; localTrace = null }

    new(message: string, localTrace: StackTrace) =
        { message = message; innerError = ValueNone; innerErrors = Seq.empty; isAggregate = false; trace = localTrace.ToString(); localTrace = localTrace }

    new(message: string, source: IError, localTrace: StackTrace) =
        { message = message; innerError = ValueSome source; innerErrors = Seq.singleton source; isAggregate = false; trace = localTrace.ToString(); localTrace = localTrace }

    new(message: string, sources: IError seq, localTrace: StackTrace) =
        { message = message; innerError = Utils.tryFirst sources; innerErrors = sources; isAggregate = true; trace = localTrace.ToString(); localTrace = localTrace }

    interface IError with
        member this.Message = this.message
        member this.InnerError = this.innerError

    interface IAggregateError with
        member this.IsAggregate = this.isAggregate
        member this.InnerErrors = this.innerErrors

    interface ITracedError with
        member this.StackTrace = this.trace
        member this.LocalStackTrace = this.localTrace
