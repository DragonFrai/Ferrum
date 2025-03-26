namespace Ferrum

open System.Diagnostics
open Microsoft.FSharp.Core


[<RequireQualifiedAccess>]
module Errors =

    [<Sealed>]
    type Empty private () =
        static member Instance = Empty()
        override this.ToString() = ""
        interface ILocalError with
            member this.Reason = ""
            member this.Source = ValueNone
            member this.StackTrace = ValueNone
            member this.LocalStackTrace = ValueNone

    [<Sealed>]
    type Message(reason: string) =
        override this.ToString() = reason
        interface ILocalError with
            member this.Reason = reason
            member this.Source = ValueNone
            member this.StackTrace = ValueNone
            member this.LocalStackTrace = ValueNone

    [<Sealed>]
    type MessageTrace(reason: string, stackTrace: StackTrace) =
        [<StackTraceHidden>]
        new(reason: string) = MessageTrace(reason, StackTrace(0, true))
        override this.ToString() = reason
        interface ILocalError with
            member this.Reason = reason
            member this.Source = ValueNone
            member this.StackTrace = ValueSome (stackTrace.ToString())
            member this.LocalStackTrace = ValueSome stackTrace

    [<Sealed>]
    type Context(context: string, source: IError) =
        override this.ToString() = context
        interface ILocalError with
            member this.Reason = context
            member this.Source = ValueSome source
            member this.StackTrace = ValueNone
            member this.LocalStackTrace = ValueNone

    [<Sealed>]
    type ContextTrace(context: string, source: IError, stackTrace: StackTrace) =
        [<StackTraceHidden>]
        new(reason: string, source: IError) = ContextTrace(reason, source, StackTrace(0, true))
        override this.ToString() = context
        interface ILocalError with
            member this.Reason = context
            member this.Source = ValueSome source
            member this.StackTrace = ValueSome (stackTrace.ToString())
            member this.LocalStackTrace = ValueSome stackTrace

    [<Sealed>]
    type Wrap<'e>(error: 'e) =
        override this.ToString() = $"{error}"
        interface ILocalError with
            member this.Reason = $"{error}"
            member this.Source = ValueNone
            member this.StackTrace = ValueNone
            member this.LocalStackTrace = ValueNone

    [<Sealed>]
    type WrapTrace<'e>(error: 'e, stackTrace: StackTrace) =
        [<StackTraceHidden>]
        new(error: 'e) = WrapTrace(error, StackTrace(0, true))
        override this.ToString() = error.ToString()
        interface ILocalError with
            member this.Reason = error.ToString()
            member this.Source = ValueNone
            member this.StackTrace = ValueSome (stackTrace.ToString())
            member this.LocalStackTrace = ValueSome stackTrace


[<AutoOpen>]
module BaseErrorsExtensions =

    type IError with

        member inline this.Context(context: string) : IError =
            Errors.Context(context, this)

        [<StackTraceHidden>]
        member inline this.ContextT(context: string) : IError =
            Errors.ContextTrace(context, this)

    [<RequireQualifiedAccess>]
    module Error =

        let inline message (reason: string) : IError =
            Errors.Message(reason)

        [<StackTraceHidden>]
        let inline messageT (reason: string) : IError =
            Errors.MessageTrace(reason)

        let inline context (context: string) (source: IError) : IError =
            Errors.Context(context, source)

        [<StackTraceHidden>]
        let inline contextT (context: string) (source: IError) : IError =
            Errors.ContextTrace(context, source)

        let inline wrap (error: 'e) : IError =
            Errors.Wrap(error)

        [<StackTraceHidden>]
        let inline wrapT (error: 'e) : IError =
            Errors.WrapTrace(error)

    [<RequireQualifiedAccess>]
    module Result =

        let message (message: string) : Result<'a, IError> =
            Error (Error.message message)

        [<StackTraceHidden>]
        let messageT (message: string) : Result<'a, IError> =
            Error (Error.messageT message)

        let context (context: string) (result: Result<'a, IError>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.context context err)

        [<StackTraceHidden>]
        let contextT (context: string) (result: Result<'a, IError>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.contextT context err)

        let wrap (result: Result<'a, 'e>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.wrap err)

        [<StackTraceHidden>]
        let wrapT (result: Result<'a, 'e>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.wrapT err)
