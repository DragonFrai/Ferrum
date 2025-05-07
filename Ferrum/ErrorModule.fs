namespace Ferrum

open System
open System.Diagnostics
open Ferrum.Internal


[<RequireQualifiedAccess>]
module Error =

    let inline message (err: IError) : string =
        err.Message

    let inline innerError (err: IError) : IError voption =
        err.InnerError

    let inline anyhow (message: string) : IError =
        MessageError(message)

    [<StackTraceHidden>]
    let inline anyhowT (message: string) : IError =
        MessageTracedError(message)

    let inline context (context: string) (source: IError) : IError =
        ContextError(context, source)

    [<StackTraceHidden>]
    let inline contextT (context: string) (source: IError) : IError =
        ContextTracedError(context, source)

    let inline wrap (error: 'e) : IError =
        WrappedError(error)

    [<StackTraceHidden>]
    let inline wrapT (error: 'e) : IError =
        WrappedTracedError(error)

    let inline aggregate (message: string) (errors: IError seq) : AggregateError =
        AggregateError(message, errors)

    [<StackTraceHidden>]
    let inline aggregateT (message: string) (errors: IError seq) : AggregateError =
        AggregateTracedError(message, errors)

    let isAggregate (err: IError) : bool =
        match err with
        | :? IAggregateError as err ->
            err.IsAggregate
        | _ ->
            false

    let sources (err: IError) : IError seq =
        match err with
        | :? IAggregateError as err ->
            err.InnerErrors
        | err ->
            match err.InnerError with
            | ValueNone -> Seq.empty
            | ValueSome source -> Seq.singleton source

    let chain (err: IError) : IError seq =
        seq {
            let rec loop (current: IError voption) = seq {
                match current with
                | ValueNone -> ()
                | ValueSome err ->
                    yield err
                    yield! loop err.InnerError
            }
            yield! loop (ValueSome err)
        }

    let getRoot (err: IError) : IError =
        let rec loop (current: IError) =
            match current.InnerError with
            | ValueNone -> current
            | ValueSome err -> loop err
        loop err

    let stackTrace (error: IError) : string voption =
        Utils.stackTraceChecked error |> ValueOption.ofObj

    let localStackTrace (error: IError) : StackTrace voption =
        Utils.localStackTraceChecked error |> ValueOption.ofObj

    let ofException (ex: exn) : IError =
        ExceptionError(ex)

    let toException (err: IError) : ErrorException =
        ErrorException(err)

    let throw<'a> (err: IError) : 'a =
        raise (ErrorException(err))

    let format (formatter: IErrorFormatter) (err: IError) : string =
        formatter.Format(err)

    let formatBy (format: string) (err: IError) : string =
        (ErrorFormatter.byFormat format).Format(err)
