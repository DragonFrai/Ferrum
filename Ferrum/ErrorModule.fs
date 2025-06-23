namespace Ferrum

open System
open System.Diagnostics
open Ferrum.Internal


[<RequireQualifiedAccess>]
module Error =

    let inline message (err: IError) : string =
        err.Message

    let innerError (err: IError) : IError option =
        err.InnerError |> Option.ofObj

    let inline failure (message: string) : IError =
        MessageError(message)

    [<StackTraceHidden>]
    let inline failureTraced (message: string) : IError =
        MessageTracedError(message)

    let inline context (context: string) (source: IError) : IError =
        ContextError(context, source)

    [<StackTraceHidden>]
    let inline contextTraced (context: string) (source: IError) : IError =
        ContextTracedError(context, source)

    let inline aggregate (message: string) (errors: IError seq) : AggregateError =
        AggregateError(message, errors)

    [<StackTraceHidden>]
    let inline aggregateTraced (message: string) (errors: IError seq) : AggregateError =
        AggregateTracedError(message, errors)

    let isAggregate (err: IError) : bool =
        match err with
        | :? IAggregateError as err ->
            err.IsAggregate
        | _ ->
            false

    /// <summary>
    /// Returns all inner errors.
    /// If error is IAggregateError returns <see cref="IAggregateError.InnerErrors"/>
    /// else returns seq with single <see cref="Error.InnerError"/> element.
    /// </summary>
    /// <param name="error"></param>
    /// <example>
    /// Error.failure "Err" |> Error.innerErrors // = [ ] <br/>
    /// Error.context "Err2" (Error.failure "Err1") |> Error.innerErrors // = [ Error.failure "Err1" ] <br/>
    /// Error.aggregate "Agg" [ Error.failure "Err1"; Error.failure "Err2" ]
    /// |> Error.innerErrors // = [ Error.failure "Err1"; Error.failure "Err2" ] <br/>
    /// </example>
    let innerErrors (error: IError) : IError seq =
        match error with
        | :? IAggregateError as err ->
            err.InnerErrors
        | err ->
            match err.InnerError with
            | null -> Seq.empty
            | source -> Seq.singleton source

    let chain (err: IError) : IError seq =
        seq {
            let rec loop (current: IError) = seq {
                match current with
                | null -> ()
                | err ->
                    yield err
                    yield! loop err.InnerError
            }
            yield! loop err
        }

    let getRoot (err: IError) : IError =
        let rec loop (current: IError) =
            match current.InnerError with
            | null -> current
            | err -> loop err
        loop err

    let stackTrace (error: IError) : string option =
        Utils.stackTraceChecked error |> Option.ofObj

    let localStackTrace (error: IError) : StackTrace option =
        Utils.localStackTraceChecked error |> Option.ofObj

    let ofException (ex: exn) : IError =
        ExceptionError(ex)

    let toException (err: IError) : ErrorException =
        ErrorException(err)

    let raise<'a> (err: IError) : 'a =
        raise (ErrorException(err))

    /// <summary>
    /// Format error using formatter
    /// </summary>
    /// <param name="formatter"></param>
    /// <param name="error"></param>
    let format (formatter: IErrorFormatter) (error: IError) : string =
        formatter.Format(error)

    /// <summary>
    /// Format error using passed format string (using base error formatters). <br/>
    /// Available format strings: <br/>
    /// "m" "M" "l1" "L1" for MessageErrorFormatter <br/>
    /// "s" "S" "l2" "L2" for SummaryErrorFormatter <br/>
    /// "d" "D" "l3" "L3" for DetailedErrorFormatter <br/>
    /// "x" "X" "l4" "L4" for DiagnosticErrorFormatter <br/>
    /// </summary>
    /// <param name="format"></param>
    /// <param name="error"></param>
    let formatBy (format: string) (error: IError) : string =
        (ErrorFormatter.byFormat format).Format(error)

    let formatL (level: int) (error: IError) : string =
        (ErrorFormatter.byLevel level).Format(error)

    /// <summary>
    /// Format using <see cref="MessageErrorFormatter"/>. <br/>
    /// Example: <c> Final error </c>
    /// </summary>
    /// <param name="error"></param>
    let formatM (error: IError) : string =
        formatBy "m" error

    /// <summary>
    /// Format using <see cref="SummaryErrorFormatter"/>. <br/>
    /// Example: <c> Final error: Middle error: Root error </c>
    /// </summary>
    /// <param name="error"></param>
    let formatS (error: IError) : string =
        formatBy "s" error

    /// <summary>
    /// Format using <see cref="DetailedErrorFormatter"/>. <br/>
    /// Example:
    /// <code>
    /// Error: Final error
    /// Cause: Middle error
    /// Cause: Root error
    ///    at ... (stack trace)
    /// </code>
    /// </summary>
    /// <param name="error"></param>
    let formatD (error: IError) : string =
        formatBy "d" error

    /// <summary>
    /// Format using <see cref="DiagnosticErrorFormatter"/>. <br/>
    /// Example:
    /// <code>
    /// Error: Final error
    ///    at ... (stack trace)
    /// Cause: Middle error
    ///    at ... (stack trace)
    /// Cause: Root error
    ///    at ... (stack trace)
    /// </code>
    /// </summary>
    /// <param name="error"></param>
    let formatX (error: IError) : string =
        formatBy "x" error

    let box (error: 'e) : IError =
        match Operators.box error with
        | :? IError as error -> error
        | :? Exception as ex -> ofException ex
        | error -> WrappedError(error)

    [<StackTraceHidden>]
    let boxTraced (error: 'e) : IError =
        match Operators.box error with
        | :? IError as error -> error
        | :? Exception as ex -> ofException ex
        | error -> WrappedTracedError(error)
