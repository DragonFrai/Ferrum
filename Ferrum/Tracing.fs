namespace Ferrum.Tracing

open System.Diagnostics
open Ferrum


/// <summary>
/// Error that can contains stack trace.
/// </summary>
[<Interface>]
type ITraceError =
    inherit IError

    /// <summary>
    ///
    /// </summary>
    /// <remarks> Not null when LocalStackTrace not null </remarks>
    /// <returns> Stack trace string or null </returns>
    abstract StackTrace: string

    /// <summary>
    ///
    /// </summary>
    /// <returns> Stack trace string or null </returns>
    abstract LocalStackTrace: StackTrace


[<AutoOpen>]
module TracingExtensions =

    [<RequireQualifiedAccess>]
    module internal Utils =
        let inline tryGetFromTraceError ([<InlineIfLambda>] getter: ITraceError -> 'a voption) (error: IError) : 'a voption =
            match error with
            | :? ITraceError as error -> getter error
            | _ -> ValueNone

    type IError with

        member this.TryGetStackTrace() : string voption =
            Utils.tryGetFromTraceError (fun error -> error.StackTrace |> ValueOption.ofObj) this

        member this.TryGetLocalStackTrace() : StackTrace voption =
            Utils.tryGetFromTraceError (fun error -> error.LocalStackTrace |> ValueOption.ofObj) this

    [<RequireQualifiedAccess>]
    module Error =

        let tryGetStackTrace (error: IError) : string voption =
            Utils.tryGetFromTraceError (fun error -> error.StackTrace |> ValueOption.ofObj) error

        let tryGetLocalStackTrace (error: IError) : string voption =
            Utils.tryGetFromTraceError (fun error -> error.StackTrace |> ValueOption.ofObj) error


[<AutoOpen>]
module TraceErrorsExtensions =

    [<RequireQualifiedAccess>]
    module Errors =

        [<Sealed>]
        type MessageTrace(reason: string, stackTrace: StackTrace) =
            [<StackTraceHidden>]
            new(reason: string) = MessageTrace(reason, StackTrace(0, true))
            override this.ToString() = reason
            interface ITraceError with
                member this.Reason = reason
                member this.Source = ValueNone
                member this.StackTrace = stackTrace.ToString()
                member this.LocalStackTrace = stackTrace

        [<Sealed>]
        type ContextTrace(context: string, source: IError, stackTrace: StackTrace) =
            [<StackTraceHidden>]
            new(reason: string, source: IError) = ContextTrace(reason, source, StackTrace(0, true))
            override this.ToString() = context
            interface ITraceError with
                member this.Reason = context
                member this.Source = ValueSome source
                member this.StackTrace = stackTrace.ToString()
                member this.LocalStackTrace = stackTrace

        [<Sealed>]
        type WrappedTrace<'e>(error: 'e, stackTrace: StackTrace) =
            [<StackTraceHidden>]
            new(error: 'e) = WrappedTrace(error, StackTrace(0, true))
            override this.ToString() = error.ToString()
            interface ITraceError with
                member this.Reason = error.ToString()
                member this.Source = ValueNone
                member this.StackTrace = stackTrace.ToString()
                member this.LocalStackTrace = stackTrace

    type IError with

        [<StackTraceHidden>]
        member inline this.ContextT(context: string) : IError =
            Errors.ContextTrace(context, this)


    [<RequireQualifiedAccess>]
    module Error =

        [<StackTraceHidden>]
        let inline messageT (reason: string) : IError =
            Errors.MessageTrace(reason)

        [<StackTraceHidden>]
        let inline contextT (context: string) (source: IError) : IError =
            Errors.ContextTrace(context, source)

        [<StackTraceHidden>]
        let inline wrapT (error: 'e) : IError =
            Errors.WrappedTrace(error)

    [<RequireQualifiedAccess>]
    module Result =

        [<StackTraceHidden>]
        let messageT (message: string) : Result<'a, IError> =
            Error (Error.messageT message)

        [<StackTraceHidden>]
        let contextT (context: string) (result: Result<'a, IError>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.contextT context err)

        [<StackTraceHidden>]
        let wrapT (result: Result<'a, 'e>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.wrapT err)

