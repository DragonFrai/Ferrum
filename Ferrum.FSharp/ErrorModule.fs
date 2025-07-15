namespace Ferrum.FSharp

open System
open System.Diagnostics

open FSharp.Core
open Ferrum
open Ferrum.Errors
open Ferrum.ExceptionInterop
open Ferrum.Formatting


[<RequireQualifiedAccess>]
module Error =

    let message (err: IError) : string =
        match err.Message with null -> String.Empty | msg -> msg

    let innerError (err: IError) : IError option =
        err.InnerError |> Option.ofObj

    let inline failure (message: string) : IError =
        MessageError(message)

    [<StackTraceHidden>]
    let inline failureTraced (message: string) : IError =
        TracedMessageError(message)

    let inline context (message: string) (source: IError) : IError =
        ContextError(message, source)

    [<StackTraceHidden>]
    let inline contextTraced (message: string) (innerError: IError) : IError =
        TracedContextError(message, innerError)

    let inline aggregate (message: string) (errors: IError seq) : IError =
        AggregateError(message, errors)

    [<StackTraceHidden>]
    let inline aggregateTraced (message: string) (innerErrors: IError seq) : IError =
        TracedAggregateError(message, innerErrors)

    let inline isAggregate (err: IError) : bool =
        err.GetIsAggregate()

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
    let inline innerErrors (error: IError) : IError seq =
        error.GetInnerErrors()

    let inline chain (error: IError) : IError seq =
        error.Chain()

    let inline getRoot (error: IError) : IError =
        error.GetRoot()

    let stackTrace (error: IError) : string option =
        error.GetStackTrace() |> Option.ofObj

    let localStackTrace (error: IError) : StackTrace option =
        error.GetLocalStackTrace() |> Option.ofObj

    let ofException (ex: exn) : IError =
        ExceptionError(ex)

    let toException (err: IError) : ErrorException =
        ErrorException(err)

    let raise<'a> (err: IError) : 'a =
        do err.Throw()
        Unchecked.defaultof<'a>

    /// <summary>
    /// Format error using formatter
    /// </summary>
    /// <param name="formatter"></param>
    /// <param name="error"></param>
    let inline format (formatter: IErrorFormatter) (error: IError) : string =
        error.Format(formatter)

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
    let inline formatBy (format: string) (error: IError) : string =
        error.Format(format)

    let inline formatL (level: int) (error: IError) : string =
        error.Format(level)

    /// <summary>
    /// Format using <see cref="MessageErrorFormatter"/>. <br/>
    /// Example: <c> Final error </c>
    /// </summary>
    /// <param name="error"></param>
    let inline formatM (error: IError) : string =
        error.FormatM()

    /// <summary>
    /// Format using <see cref="SummaryErrorFormatter"/>. <br/>
    /// Example: <c> Final error: Middle error: Root error </c>
    /// </summary>
    /// <param name="error"></param>
    let inline formatS (error: IError) : string =
        error.FormatS()

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
    let inline formatD (error: IError) : string =
        error.FormatD()

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
    let inline formatX (error: IError) : string =
        error.FormatX()

    let inline box (error: 'e) : IError =
        ValueError(error)

    let inline boxWith (toString: 'e -> string) (error: 'e) : IError =
        ValueError(error, toString)

    [<StackTraceHidden>]
    let inline boxTraced (error: 'e) : IError =
        TracedValueError(error)

    [<StackTraceHidden>]
    let inline boxWithTraced (toString: 'e -> string) (error: 'e) : IError =
        TracedValueError(error, toString)
