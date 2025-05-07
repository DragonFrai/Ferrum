# Ferrum

[![Nuget](https://img.shields.io/nuget/vpre/Ferrum)](https://www.nuget.org/packages/Ferrum/)

The F# library for working with dynamic Error types.

The first library goal is providing lightweight general dynamic 
error type `IError` that can be used in `Result` 
error case (`Result<'a, IError>`) instead `string` or `exn` (`Exception`).
The `string` error type does not compose well;
`exn` is composable, but it is heavier and looks strange when not thrown.
`IError` type is something in between.

The second library goal is providing contextual enrichment for errors.


## Examples


### IError usages

Ferrum provides 3 fundamental error types: `message`, `context`, `wrap`.

- `message` is error that is created first and causes the all errors chain.
- `context` is error that add context to other error.
- `wrap` is error that wrap any `'e` type to `IError`. 
  It useful if impossible or too expensive to implement `IError` interface for `'e`.
  `'e` can be exception, but recommended use `Error.ofException` for converting `exn -> IError`.

```fsharp
let messageError: IError = Error.message "This is some error"
printfn $" > {messageError.FormatChain()}"
// > This is some error

let contextualError: IError = Error.context "Top error" (Error.message "Root error")
printfn $" > {contextualError.FormatChain()}"
// > Top error: Root error

type SomeError = SomeError
let wrappedError: IError = Error.wrap SomeError
printfn $" > {wrappedError.FormatChain()}"
// > SomeError
```

All operation are duplicated on Result module:

```fsharp
let messageResult: Result<unit, IError> = Result.message "This is some error"
printfn $" > {Utils.formatResultChain messageResult}"
// > Error (This is some error)

let contextualResult: Result<unit, IError> = Result.context "Top error" (Result.message "Root error")
printfn $" > {Utils.formatResultChain contextualResult}"
// > Error (Top error: Root error)

type SomeError = SomeError
let wrappedResult: Result<unit, IError> = Result.wrap (Error SomeError)
printfn $" > {Utils.formatResultChain wrappedResult}"
// > Error (SomeError)
```


### Complex usage

```fsharp
[<RequireQualifiedAccess>]
type IOError =
    | FileNotFound

let readFile (_fileName: string) : Result<string, IOError> =
    Error IOError.FileNotFound

let makeGreeting () : Result<string, IError> =
    let readNameResult = readFile "name.txt" |> Result.wrap |> Result.context "Name is unknown"
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
printfn $" > {Utils.formatResultChain res}"
// > Error (Greeting is not build: Name is unknown: FileNotFound)

```


### Custom IError implementation

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
printfn $" > {simpleError.Format(ErrorFormatters.ChainErrorFormatter.Instance)}"
// > Some simple error case

let complexErrorSomeError = ComplexError.SomeError
printfn $" > {complexErrorSomeError.Format(ErrorFormatters.ChainErrorFormatter.Instance)}"
// > Some complex error case

let complexErrorWithSource = ComplexError.Source SimpleError.SimpleCase
printfn $" > {complexErrorWithSource.Format(ErrorFormatters.ChainErrorFormatter.Instance)}"
// > Error caused by simple error source: Some simple error case
```

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

