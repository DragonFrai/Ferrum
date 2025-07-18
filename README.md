# Ferrum

[![Nuget](https://img.shields.io/nuget/vpre/Ferrum)](https://www.nuget.org/packages/Ferrum/)

The F# library for working with dynamic Error types.

The first library goal is providing lightweight general dynamic error type.
The second library goal is providing base tools for working with this error type.

Ferrum define `IError` type that can be used in `Result` error case 
(`Result<'a, IError>`) instead `string` or `exn` (`Exception`).
The `string` error type does not compose well.
The `exn` is composable, but it is heavier and looks strange when not thrown.
`IError` type is something in between.


## Basic errors

Ferrum provides 2 fundamental error types `MessageError` and `ContextError`.
- `Message` is error that contains root cause of the error.
- `Context` is error that add context to other error.

Composing this error types forms errors chain like exceptions combines.

Functions `Error.message/Result.message` and `Error.context/Result.context`
can be used for creating its errors.

```fsharp
let messageError: IError = Error.message "This is some error"
printfn $"> {Error.formatS messageError}"
//> This is some error

let contextualError: IError = Error.context "Final error" (Error.message "Root error")
printfn $"> {Error.formatS contextualError}"
//> Final error: Root error
```


## Converting from/to exceptions

For converting from/to exception used functions 
`Error.ofException`/`Error.toException` and `Result.ofExnResult/Result.toExnResult`.

```fsharp
let ex = exn("Some exn", exn("Inner exn"))
let err = Error.ofException ex
printfn $"{Error.formatS err}"
// Some exn: Inner exn

let err = Error.context "Some error" (Error.message "Inner error")
let ex = Error.toException err
printfn $"{ex}"
// Ferrum.ErrorException: Some error
//  ---> Ferrum.ErrorException: Inner error
//    --- End of inner exception stack trace ---
```


## Converting not IError types to IError

For converting existed errors that not implements `IError` interface used `WrappedError` type.
Recommends use `Error.box` and `Result.boxError` functions for converting some `'a` type to `IError` type.

This functions keep `'a :> IError` the same and converts exceptions using `Error.ofException`;
other types wraps to `WrapperError` that use `ToString` for error message.
Wrapped errors can not contain inner errors.

```fsharp
type MyError = | MyError

let wrappedError = Error.box MyError
printfn $"{Error.formatS wrappedError}"
// MyError

let contextError = Error.context "Some context" wrappedError
printfn $"{Error.formatS contextError}"
// Some context: MyError

```


## Error formatting

Interface `IErrorFormatter` used for implement error formatters 
and can apply to `IError` using `Error.format formatter error`.

Default formatters is `MessageErrorFormatter`, `SummaryErrorFormatter`,
`DetailedErrorFormatter` and `DiagnosticErrorFormatter`.

All formatters has singleton instance (`XErrorFormatter.Instance`)
but fo formatting exist functions `Error.formatBy` that can be used with
format string and corresponding shortcuts `Error.formatX`. 
Formatters, format strings and shortcuts shown in the table below.

| Formatter                | Format | Level | Function shortcut | Messages  | Lines     | Stack trace |
|--------------------------|--------|-------|-------------------|-----------|-----------|-------------|
| MessageErrorFormatter    | m/l1   | 1     | Error.formatM     | Top level | Single    | No          |
| SummaryErrorFormatter    | s/l2   | 2     | Error.formatS     | All       | Single    | No          |
| DetailedErrorFormatter   | d/l3   | 3     | Error.formatD     | All       | Multiline | Root error  |
| DiagnosticErrorFormatter | x/l4   | 4     | Error.formatX     | All       | Multiline | All errors  |

For formatting by level used `Error.formatL` function.

Remark: Errors provided Ferrum implements IFormattable, 
but external errors that implements IError can be not IFormattable.
So it is recommended to format errors explicitly using functions (`Error.formatS`), 
not using interpolation args (`$"{error:s}"`).

(In version 0.3 not implemented FormatProvider for errors)


## Complex usage example

```fsharp
[<RequireQualifiedAccess>]
type IOError =
    | FileNotFound

let readFile (_fileName: string) : Result<string, IOError> =
    Error IOError.FileNotFound

let makeGreeting () : Result<string, IError> =
    let readNameResult = readFile "name.txt" |> Result.boxError |> Result.context "Name is unknown"
    match readNameResult with
    | Error err -> Error err
    | Ok name -> Ok $"Hello, {name}"

let greet () : Result<unit, IError> =
    let makeGreetingResult = makeGreeting () |> Result.context "Greeting is not build"
    match makeGreetingResult with
    | Error err -> Error err
    | Ok greeting ->
        printfn $"{greeting}"
        Ok ()

let res: Result<unit, IError> = greet ()
printfn $" > {Utils.formatResult Error.formatS res}"
// > Error (Greeting is not build: Name is unknown: FileNotFound)

```


## Custom IError implementation

```fsharp
type SimpleError =
    | SimpleCase
    with
        interface IError with
            member this.Message =
                match this with
                | SimpleCase -> "Some simple error case"
            member this.InnerError =
                ValueNone

type ComplexError =
    | Source of SimpleError
    | SomeError
    with
        interface IError with
            member this.Message =
                match this with
                | Source _ -> "Error caused by simple error source"
                | SomeError -> "Some complex error case"
            member this.InnerError =
                match this with
                | Source simpleError -> ValueSome simpleError
                | SomeError -> ValueNone

let simpleError = SimpleError.SimpleCase
printfn $" > {simpleError |> Error.formatS}"
// > Some simple error case

let complexErrorSomeError = ComplexError.SomeError
printfn $" > {complexErrorSomeError |> Error.formatS}"
// > Some complex error case

let complexErrorWithSource = ComplexError.Source SimpleError.SimpleCase
printfn $" > {complexErrorWithSource |> Error.formatS}"
// > Error caused by simple error source: Some simple error case
```


## Stack tracing

Ferrum provides `IError` subtype `ITracedError` that can contains stack trace.
For accessing to traces without explicit type checking,
used functions `Error.stackTrace: IError -> string` and `Error.localStackTrace: IError -> StackTrace`.

Unlike exceptions, errors cannot be "thrown". 
Errors is a values that creates once and moving to different places.
It's requires explicit adding tracing inside error value.

For creating `IError` with stack trace, provided functions 
`Error.messageTraced`, `Error.contextTraced`, `Error.aggregateTraced`, `Error.boxTraced`,
`Result.messageTraced`, `Result.contextTraced`, `Result.aggregateTraced`, `Result.boxErrorTraced`.

All functions above always add stack trace to error. 
Ferrum does not provide dynamic tracing or not error based on environment.
If it is preferred for you add tracing or not depending on environment 
(for example environment variables or build configuration)
then add your own wrapper.

In addition, it is impossible to say exactly which errors need to be traced.
You can add tracing only for root errors, because its nearest to fault point 
and in most cases context is higher up the call stack. 
But this rule does not work so well for errors ware passed between workers, 
because call stack becomes decoupled and both faults points can be important.

Remark: `Error.boxTraced` and `Result.boxErrorTraced` does not add new stack trace if
boxable type already contains stack trace.


## Error aggregating

See `Error.aggregate`, `Error.innerErrors`

TBD


## When should use IError?

Erasing error details useful for `not domain` code, 
which does not care about error cases and their details.

Using a common error interface is useful not only for unification,
but also for defining a error structure. Ferrum interfaces allows to 
create error chains with explicit division of levels.
(Ideally, interface like this should be in FSharp.Core)

For "domain" code, including libraries, the implementation of `IError` and inheritors 
also allows to uncover the structure without specifying type explicitly 
(and also cast to the general type without boxing).


## Why not Exceptions?

Exceptions have all the properties that IErrors have.
Literally Message <=> Message, InnerError <=> InnerException, StackTrace <=> StackTrace. 
The only difference is that IError is a bit more focused on using in Result 
and is possibly more lightweight.
It is easy to implement a similar utilities for exn. 
It all comes down to stylistic preferences.


## Additional Result and Error functions

At the moment, Ferrum has no goals to cover the usability of 
error handling beyond the universal error type. 
FsToolkit.ErrorHandling and other libraries do a great job with this.


## Library stability

At version 0.3 Ferrum pretty experimental.
First of all, this applies to the structure of interfaces and their member types
(for reasons of optimization and not bloating IError)
However, the Error module should be the most stable unless there are compelling reasons for major changes.

## References

I'd like to add here one of the pages of thinking about error types from the Rust ecosystem. 
I find them to be common, except for the language barrier. You can also find descriptions 
of different approaches to implementing a specific error type.
[Error design](https://nrc.github.io/error-docs/error-design/index.html)
