namespace Ferrum

open System
open System.Diagnostics
open Ferrum.Internal


[<RequireQualifiedAccess>]
module Error =

    let inline reason (err: IError) : string =
        err.Reason

    let inline source (err: IError) : IError voption =
        err.Source

    let inline message (reason: string) : IError =
        MessageError(reason)

    [<StackTraceHidden>]
    let inline messageT (reason: string) : IError =
        MessageTracedError(reason)

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

    let inline aggregate (reason: string) (errors: IError seq) : AggregateError =
        AggregateError(reason, errors)

    [<StackTraceHidden>]
    let inline aggregateT (reason: string) (errors: IError seq) : AggregateError =
        AggregateTracedError(reason, errors)

    let sources (err: IError) : IError seq =
        match err with
        | :? IAggregateError as err ->
            err.Sources
        | err ->
            match err.Source with
            | ValueNone -> Seq.empty
            | ValueSome source -> Seq.singleton source

    let chain (err: IError) : IError seq =
        seq {
            let rec loop (current: IError voption) = seq {
                match current with
                | ValueNone -> ()
                | ValueSome err ->
                    yield err
                    yield! loop err.Source
            }
            yield! loop (ValueSome err)
        }

    let getRoot (err: IError) : IError =
        let rec loop (current: IError) =
            match current.Source with
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
        let formatter: IErrorFormatter =
            match format with
            | null | "" -> FinalErrorFormatter.Instance // TODO?: Use ChainShortenedErrorFormatter as default?
            | "f" -> FinalErrorFormatter.Instance
            | "c" -> ChainErrorFormatter.Instance
            | "F" -> FinalMultilineErrorFormatter.Instance
            | "C" -> ChainMultilineErrorFormatter.Instance
            | "S" -> ChainShortenedErrorFormatter.Instance
            | _ -> raise (FormatException($"The {format} format string is not supported."))
        formatter.Format(err)

    let formatFinal (err: IError) : string =
        (FinalErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let formatFinalMultiline (err: IError) : string =
        (FinalMultilineErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let formatChain (err: IError) : string =
        (ChainErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let formatChainShortened (err: IError) : string =
        (ChainShortenedErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let formatChainMultiline (err: IError) : string =
        (ChainMultilineErrorFormatter.Instance :> IErrorFormatter).Format(err)
