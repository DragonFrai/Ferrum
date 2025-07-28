# Convert exceptions to errors

For convert `Exception` to `IError` used extension member `Exception.ToError` which has the following behavior:
- If the exception has inner exception, then the returned error has inner error based on this inner exception
- If the exception is aggregated, then the returned error is also aggregated
- If the exception has stack trace, then the returned error has stack trace


Simple example:

```csharp
Exception ex = new Exception("Simple exception");
IError error = ex.ToError();

Assert.Equal("Simple exception", error.Message);
Assert.Null(error.InnerError);
```


With inner exception:

```csharp
Exception ex = new Exception("Final exception", new Exception("Inner exception"));
IError error = ex.ToError();

Assert.Equal("Final exception", error.Message);
Assert.Equal("Inner exception", error.InnerError!.Message);
```


With aggregation:

```csharp
Exception ex = new AggregateException(
    "Aggregate exception",
    new Exception("Inner exception 0"),
    new Exception("Inner exception 1")
);
IError error = ex.ToError();
IError[] innerErrors = error.GetInnerErrors().ToArray();

Assert.Equal("Aggregate exception (Inner 0) (Inner 1)", error.Message);
Assert.Equal("Inner exception 0", innerErrors[0].Message);
Assert.Equal("Inner exception 1", innerErrors[1].Message);
```


With stack trace:

```csharp
IError error;
try
{
    throw new Exception("Traced exception");
}
catch (Exception ex)
{
    error = ex.ToError();
}

Assert.Equal("Exception msg", error.Message);
Assert.Null(error.InnerError);
Assert.NotNull(error.GetStackTrace());
```
