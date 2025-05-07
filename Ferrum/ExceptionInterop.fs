namespace Ferrum

open System
open Ferrum
open Ferrum.Internal


type ErrorException =
    inherit Exception

    val private _error: IError

    new(error: IError) =
        let innerException: exn =
            match error.InnerError with
            | ValueNone -> null
            | ValueSome source -> ErrorException(source)
        { inherit Exception(error.Message, innerException)
          _error = error }

    member this.Error: IError = this._error

    override this.StackTrace = Utils.stackTraceChecked this.Error


type ExceptionError =

    val private _source: IError voption
    val private _exception: exn

    new(exception': exn) =
        let sourceExn =
            match exception'.InnerException with
            | null -> ValueNone
            | inner -> ValueSome(ExceptionError(inner) :> IError)
        { _exception = exception'
          _source = sourceExn }

    member this.Exception: exn = this._exception

    override this.ToString() = this._exception.Message

    interface ITracedError with
        member this.Message = this._exception.Message
        member this.InnerError = this._source
        member this.StackTrace = this._exception.StackTrace
        member this.LocalStackTrace = null
