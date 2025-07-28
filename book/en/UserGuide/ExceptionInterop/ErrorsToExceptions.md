
## Converting errors to exceptions

For convert `IError` to `Exception` used extension member `IError.ToException` which has the following behavior:
- If the error has inner error, then the returned exception has inner exception based on this inner error
- If the error is aggregated, then the returned exception is also aggregated
- The stack trace of the returned exception is not overwritten by the error stack trace

<div class="warning"> 
The stack trace of the returned exception is not overwritten by the error stack trace.
</div>


Simple example:

```csharp
IError error = new MessageError("Error msg");
Exception ex = error.ToException();

Assert.Equal("Error msg", ex.Message);
Assert.Null(ex.InnerException);
```


With inner errors:
```csharp
IError error = new ContextError("Error msg", new MessageError("Inner msg"));
Exception ex = error.ToException();

Assert.Equal("Error msg", ex.Message);
Assert.Equal("Inner msg", ex.InnerException!.Message);
```


With aggregation:
```csharp
IError error = new AggregateError(
    "Aggregate error",
    new MessageError("Inner 0"),
    new MessageError("Inner 1")
);
Exception ex = error.ToException();
Exception[] innerEx = ((AggregateException) ex).InnerExceptions.ToArray();

Assert.Equal("Aggregate error (Inner 0) (Inner 1)", ex.Message);
Assert.Equal("Inner 0", innerEx[0].Message);
Assert.Equal("Inner 1", innerEx[1].Message);
```


With stack trace:

```csharp
IError error = new TracedMessageError("Traced error msg");
Assert.NotNull(error.GetStackTrace());
Exception ex = error.ToException();

Assert.Null(ex.StackTrace); // Error stack trace does not mirror to exception
```
