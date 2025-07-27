# Basic Creating and Composing


You can create "non discriminated" errors using the types provided by `Ferrum`.


To create an error that contains only message, use `MessageError`:

```csharp
IError error = new MessageError("Message based error");

Debug.Assert(error.Message == "Message based error");
Debug.Assert(error.InnerError == null);
```


Add high-level context to errors to understand where and how low-level errors happen.
Use `ContextError` class or `error.Context(string)` extension:

```csharp
IError cause = new MessageError("Main error");
IError error = new ContextError("Context error", cause);

Debug.Assert(error.Message == "Context error");
Debug.Assert(error.InnerError!.Message == "Main error");
```


All types that are errors could implement `IError`, but this is not always possible.
`AnyError.OfValue` converts any type to an `IError`. By default, the result of `ToString`
is used for the error message, but some types converted in a more idiomatic way.
You should not depend on the specific behavior of `AnyError.OfValue`.

```csharp
(string, int, int) notIErrorError = ("invalid token", 12, 48);
IError error = AnyError.OfValue(notIErrorError);

Assert.Equal("(invalid token, 12, 48)", error.Message);
```

The `error.Chain()` extension gets the entire chain of errors as an enumerator,
and `error.GetRoot()` gets the root error (the last error in the chain).
