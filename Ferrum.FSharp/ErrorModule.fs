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

    let getMessage (err: IError) : string =
        match err.Message with null -> String.Empty | msg -> msg

    let getInnerError (err: IError) : IError option =
        err.InnerError |> Option.ofObj

    let inline message (message: string) : IError =
        MessageError(message)

    [<StackTraceHidden>]
    let inline messageTraced (message: string) : IError =
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

    /// <summary>
    /// <see cref="Ferrum.ErrorExtensions.GetIsAggregate"/>
    /// </summary>
    /// <param name="err"></param>
    let inline getIsAggregate (err: IError) : bool =
        err.GetIsAggregate()

    /// <summary>
    /// Returns all inner errors.
    /// If error is IAggregateError returns <see cref="IAggregateError.InnerErrors"/>
    /// else returns seq with single <see cref="Error.InnerError"/> element.
    /// </summary>
    /// <param name="error"></param>
    /// <example>
    /// <code>
    /// // = [ ]
    /// Error.message "Err" |> Error.getInnerErrors
    /// // = [ Error.message "Err1" ]
    /// Error.context "Err2" (Error.message "Err1") |> Error.getInnerErrors
    /// // = [ Error.message "Err1"; Error.message "Err2" ]
    /// Error.aggregate "Agg" [ Error.message "Err1"; Error.message "Err2" ] |> Error.getInnerErrors
    /// </code>
    /// </example>
    let inline getInnerErrors (error: IError) : IError seq =
        error.GetInnerErrors()

    let inline chain (error: IError) : IError seq =
        error.Chain()

    let inline getRoot (error: IError) : IError =
        error.GetRoot()

    let getStackTrace (error: IError) : string option =
        error.GetStackTrace() |> Option.ofObj

    let ofException (ex: exn) : IError =
        ex.ToError()

    let toException (err: IError) : Exception =
        err.ToException()

    // TODO?: Add ValueError creating
    //
    // NOTE:
    //   The ValueError type looks pretty "technically".
    //   Actually ValueError used for call ToString method or user function X -> String.
    //   Instantiating this error type looks delicate and should be used is specific scenarios.
    //   It does not mean that this types must not be created by Error and Result modules functions,
    //   but "delicate" makes adding a bit careless.
    //
    // let inline ofValue (error: 'e) : IError =
    //     ValueError(error)
    //
    // let inline ofValueWith (toString: 'e -> string) (error: 'e) : IError =
    //     ValueError(error, toString)
    //
    // [<StackTraceHidden>]
    // let inline ofValueTraced (error: 'e) : IError =
    //     TracedValueError(error)
    //
    // [<StackTraceHidden>]
    // let inline ofValueWithTraced (toString: 'e -> string) (error: 'e) : IError =
    //     TracedValueError(error, toString)

    let inline box (any: 'e) : IError =
        AnyError.Create(any)

    // let raise<'a> (err: IError) : 'a =
    //     do err.Throw()
    //     Unchecked.defaultof<'a>

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

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.MessageErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    let inline formatM (error: IError) : string =
        error.FormatM()

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.SummaryErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    let inline formatS (error: IError) : string =
        error.FormatS()

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.DetailedErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    let inline formatD (error: IError) : string =
        error.FormatD()

    /// <summary> Format the error using <see cref="T:Ferrum.Formatting.Formatters.DiagnosticErrorFormatter"/> </summary>
    /// <param name="error"> Error for formatting </param>
    let inline formatX (error: IError) : string =
        error.FormatX()
