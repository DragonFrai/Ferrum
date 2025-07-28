# Creating Domain Error Types

How a domain error should be implemented is not a trivial question.
The implementation of `IError` has nothing to do with it, 
since domain errors are defined to be handled in the code itself without type erasure.
Examples of errors implementations are presented here, but the "how" answer cannot be given.


## C# Examples

```csharp
// EnumWithContextIoErrorExample

public enum IoErrorKind
{
    FileNotFound,
    PermissionDenied,
}

public class IoError(IoErrorKind kind, string path) : IError
{
    public IoErrorKind Kind { get; } = kind;
    public string Path { get; } = path;

    public string Message
    {
        get
        {
            return Kind switch
            {
                IoErrorKind.FileNotFound => $"File '{Path}' not found",
                IoErrorKind.PermissionDenied => $"File '{Path}' can't be opened",
                _ => throw new UnreachableException()
            };
        }
    }

    public IError? InnerError => null;
}
```

```csharp
// EnumOnlyIoErrorExample

public enum IoErrorKind
{
    FileNotFound,
    PermissionDenied,
}

public class IoError(IoErrorKind kind) : IError
{
    public IoErrorKind Kind { get; } = kind;

    public string Message
    {
        get
        {
            return Kind switch
            {
                IoErrorKind.FileNotFound => $"File not found",
                IoErrorKind.PermissionDenied => $"File can't be opened",
                _ => throw new UnreachableException()
            };
        }
    }

    public IError? InnerError => null;
}
```

```csharp
// InheritanceIoErrorExample

public abstract class IoError : IError
{
    public abstract string Message { get; }
    public abstract IError? InnerError { get; }
}

public class FileNotFoundIoError() : IoError
{
    public override string Message => "File not found";
    public override IError? InnerError => null;
}

public class PermissionDeniedIoError() : IoError
{
    public override string Message => "Permission denied";
    public override IError? InnerError => null;
}
```

## F# Examples

```fsharp
// EnumWithContextIoErrorExample =

[<RequireQualifiedAccess>]
type IoErrorKind = FileNotFound | PermissionDenied

type IoError =
    { Kind: IoErrorKind; Path: string }
    interface IError with
        member this.Message =
            match this.Kind with
            | IoErrorKind.FileNotFound -> $"File '{this.Path}' not found"
            | IoErrorKind.PermissionDenied -> $"File '{this.Path}' can't be opened"
        member this.InnerError = null
```

```fsharp
// EnumOnlyIoErrorExample =

[<RequireQualifiedAccess>]
type IoError =
    | FileNotFound
    | PermissionDenied
    interface IError with
        member this.Message =
            match this with
            | IoError.FileNotFound -> "File not found"
            | IoError.PermissionDenied -> "File can't be opened"
        member this.InnerError = null
```

```fsharp
// HierarchicalExample =

type SimpleError =
    | SimpleCase
    interface IError with
        member this.Message =
            match this with
            | SimpleCase -> "Some simple error case"
        member this.InnerError =
            null

type ComplexError =
    | Source of SimpleError
    | SomeError
    interface IError with
        member this.Message =
            match this with
            | Source _ -> "Error caused by simple error source"
            | SomeError -> "Some complex error case"
        member this.InnerError =
            match this with
            | Source simpleError -> simpleError
            | SomeError -> null
```

## Additional Links

I recommend the following thoughts from the Rust ecosystem if the language barrier is not an issue for you.
[Error design in Rust](https://nrc.github.io/error-docs/error-design/index.html)
