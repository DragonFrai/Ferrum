module [<AutoOpen>] Ferrum.ErrorModuleExtensions

open System.Diagnostics
open Ferrum.Formatting
open Ferrum.Internal


[<RequireQualifiedAccess>]
module Error =

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

    let segregate (error: IError) : IError seq =
        match error with
        | :? AggregateError as error -> error.Errors
        | error -> Seq.singleton error


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

    let inline ofException (ex: exn) : IError =
        ExceptionError(ex)

    let inline toException (err: IError) : ErrorException =
        ErrorException(err)

    let throw<'a> (err: IError) : 'a =
        raise (ErrorException(err))

    let inline format (formatter: IErrorFormatter) (err: IError) : string =
        formatter.Format(err)

    let inline formatFinal (err: IError) : string =
        (FinalErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let inline formatChain (err: IError) : string =
        (ChainErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let inline formatMultiline (err: IError) : string =
        (MultilineErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let inline formatMultilineTrace (err: IError) : string =
        (MultilineTraceErrorFormatter.Instance :> IErrorFormatter).Format(err)

    let inline formatMultilineTraceAll (err: IError) : string =
        (MultilineTraceAllErrorFormatter.Instance :> IErrorFormatter).Format(err)



