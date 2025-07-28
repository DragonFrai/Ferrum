# Ferrum

- **Ferrum**: 
  [![Nuget](https://img.shields.io/nuget/vpre/Ferrum)](https://www.nuget.org/packages/Ferrum/)

- **Ferrum.FSharp**:
  [![Nuget](https://img.shields.io/nuget/vpre/Ferrum.FSharp)](https://www.nuget.org/packages/Ferrum.FSharp/)

The F# + C# library for working with dynamic Error types.

**Ferrum** allows you to create errors based on the message, add context to errors, aggregate errors, and more.
In many scenarios, using error-values can be more practical than using exceptions or errors as a simple string, 
which have awkward composition methods.

Open the [**Ferrum book**](https://dragonfrai.github.io/Ferrum/) to learn how to use **Ferrum**.


## Quick start C# and F# examples


### C# without Result<TValue, TError> monad type
```csharp
using Ferrum;
using Ferrum.Errors;
using Ferrum.ExceptionInterop;
using Ferrum.Formatting;

public static class ReadmeExample
{
    public static IError? ReadFile(string path, out string content)
    {
        try
        {
            var text = File.ReadAllText(path);
            content = text;
            return null;
        }
        catch (Exception e)
        {
            content = string.Empty;
            return e.ToError();
        }
    }

    public static IError? CreateHello(out string hello)
    {
        var readError = ReadFile("name.txt", out var name);
        if (readError is not null)
        {
            hello = string.Empty;
            return new ContextError("Hello not created", readError);
        }

        hello = $"Hello, {name}!";
        return null;
    }

    public static void ItIsMain(string[] args)
    {
        var helloStringError = CreateHello(out var helloString);
        if (helloStringError is not null)
        {
            // Print:
            // I can't say hello to you: Hello not created: Could not find file '.../name.txt'.
            Console.WriteLine($"I can't say hello: {helloStringError.FormatS()}");
        }
        else
        {
            // Print:
            // I say you: Hello, <name>!
            Console.WriteLine($"I say: {helloString}");
        }
    }
}

```


### C# with dotNext

TODO


### F#

```fsharp
open System.IO
open Ferrum
open Ferrum.FSharp

module ReadmeExample =

    let readFile (path: string) : Result<string, IError> =
        try
            Ok (File.ReadAllText(path))
        with ex ->
            Error (Error.ofException ex)

    let createHello () : Result<string, IError> =
        readFile "name.txt"
        |> Result.context "Hello not created"
        |> Result.map (fun name -> $"Hello, {name}!")

    let itIsMain (args: string array) : int =
        match createHello () with
        | Error error ->
            // Print:
            // I can't say hello to you: Hello not created: Could not find file '.../name.txt'.
            printfn $"I can't say hello: {Error.formatS error}"
        | Ok helloString ->
            // Print:
            // I say you: Hello, <name>!
            printfn $"I say: {helloString}"
        0
```


## When should use Ferrum?

The concept of a dynamic error is useful for *non-domain* code 
that doesn't care about the details of errors.

However, interfaces that add some structure allow for unified post-processing 
that doesn't rely on *domain* details.

**Ferrum**'s interfaces are oriented toward (but not limited to) a chained error hierarchy,
which is most useful for adding high-level context on top of low-level context.


## Why not Exceptions?

In terms of basic properties, `Ferrum.IError` and `System.Exception` are equivalent.
Message/Message, InnerError/InnerException, StackTrace/StackTrace.
The difference is in the "emphasis".
Errors are intended to be primarily transferable and composable values.
These same properties are useful for TError in Result<TValue, TError> types.

It all comes down to stylistic preferences.


## Result<TValue, TError> type

**Ferrum** has no goals to cover the usability of 
error handling beyond the universal error type.
[dotNext](https://github.com/dotnet/dotNext),
[FsToolkit.ErrorHandling](https://github.com/demystifyfp/FsToolkit.ErrorHandling),
and other .NET libraries do a great job with this.


## Library stability

In version 0.4, **Ferrum** is pretty experimental.
Therefore, some components may be unstable. 
If we ignore the namespace and division into packages, then I would give the following stability rating:

The most stable components are `IError`, `MessageError`, `ContextError`, `IErrorFormatter` 
and related components.

Fairly stable components are `IAggregateError`, `AggregateError`, 
`ITracedError`, `TracedMessageError`, `TracedContextError`, `TracedAggregateError`, 
`MessageErrorFormatter`, `SummaryErrorFormatter`, `DetailedErrorFormatter`, `DiagnosticErrorFormatter`, 
*ExceptionErrors*, *ErrorExceptions*, `AnyError.OfValue` and related components.

The remaining components are the least stable.


## From v0.3 to v0.4 F# migration

Starting with v0.4, the core components were rewritten in C# so that non-F# .NET libraries 
could use **Ferrum** without unnecessary dependencies. 
The main **Ferrum** package became a C# package. 
To use F# integrations, use the **Ferrum.FSharp** package.

