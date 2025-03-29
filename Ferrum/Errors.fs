namespace Ferrum

open Microsoft.FSharp.Core


[<RequireQualifiedAccess>]
module Errors =

    [<Sealed>]
    type Message(reason: string) =
        override this.ToString() = reason
        interface IError with
            member this.Reason = reason
            member this.Source = ValueNone

    [<Sealed>]
    type Context(context: string, source: IError) =
        override this.ToString() = context
        interface IError with
            member this.Reason = context
            member this.Source = ValueSome source

    [<Sealed>]
    type Wrap<'e>(error: 'e) =
        override this.ToString() = $"{error}"
        interface IError with
            member this.Reason = $"{error}"
            member this.Source = ValueNone


[<AutoOpen>]
module ErrorsExtensions =

    type IError with

        member inline this.Context(context: string) : IError =
            Errors.Context(context, this)

    [<RequireQualifiedAccess>]
    module Error =

        let inline message (reason: string) : IError =
            Errors.Message(reason)

        let inline context (context: string) (source: IError) : IError =
            Errors.Context(context, source)

        let inline wrap (error: 'e) : IError =
            Errors.Wrap(error)


    [<RequireQualifiedAccess>]
    module Result =

        let message (message: string) : Result<'a, IError> =
            Error (Error.message message)

        let context (context: string) (result: Result<'a, IError>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.context context err)

        let wrap (result: Result<'a, 'e>) : Result<'a, IError> =
            match result with
            | Ok x -> Ok x
            | Error err -> Error (Error.wrap err)
