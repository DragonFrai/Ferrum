namespace Ferrum.Throwing

open System
open Ferrum
open Ferrum.Formatting


[<Sealed>]
type ErrorException(error: IError, formatter: IErrorFormatter) =
    inherit Exception(formatter.Format(error)) // TODO?: Wrap source error to inner exception 
    new(error: IError) = ErrorException(error, ErrorFormatters.TopErrorFormatter.Instance)
    member this.Error: IError = error

[<RequireQualifiedAccess>]
module Errors =

    [<Sealed>]
    type WrapException(ex: exn) =
        member this.Exception: exn = ex
        override this.ToString() = ex.Message
        interface IError with
            member this.Reason = ex.Message
            member this.Source =
                match ex.InnerException with
                | null -> ValueNone
                | inner -> ValueSome(WrapException(inner)) // TODO?: Cache it
            member this.StackTrace =
                match ex.StackTrace with
                | null -> ValueNone
                | stackTrace -> ValueSome stackTrace

[<AutoOpen>]
module ThrowingErrorExtension =

    type IError with

        static member OfException(ex: exn) : IError =
            Errors.WrapException(ex)

        member this.ToException() : ErrorException =
            ErrorException(this)

        member this.ToException(formatter: IErrorFormatter) : ErrorException =
            ErrorException(this, formatter)

        member this.Throw<'a>() : 'a =
            raise (ErrorException(this))

        member this.Throw<'a>(formatter: IErrorFormatter) : 'a =
            raise (ErrorException(this, formatter))

    [<RequireQualifiedAccess>]
    module Error =

        let ofException (ex: exn) : IError =
            Errors.WrapException(ex)

        let toException (err: IError) : ErrorException =
            ErrorException(err)

        let toExceptionInFormat (formatter: IErrorFormatter) (err: IError) : ErrorException =
            ErrorException(err, formatter)

        let throw<'a> (err: IError) : 'a =
            raise (ErrorException(err))

        let throwInFormat<'a> (formatter: IErrorFormatter) (err: IError) : 'a =
            raise (ErrorException(err, formatter))

    [<RequireQualifiedAccess>]
    module Result =
        let catchToError (f: unit -> 'a) : Result<'a, IError> =
            try Ok(f ())
            with ex -> Error (Error.ofException ex)

        let getOrThrowError (result: Result<'a, IError>) : 'a =
            match result with
            | Ok x -> x
            | Error err -> Error.throw err

        let getOrThrowErrorInFormat (errorFormatter: IErrorFormatter) (result: Result<'a, IError>) : 'a =
            match result with
            | Ok x -> x
            | Error err -> Error.throwInFormat errorFormatter err
