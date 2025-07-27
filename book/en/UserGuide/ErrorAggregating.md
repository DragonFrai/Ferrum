# Error Aggregating

Errors that may contain multiple low-level errors should implement the `IAggregateError` interface.

It extends `IError` with an `InnerErrors` property, which unlike `InnerError` is a collection of errors,
similar to `InnerExceptions` being the property of `AggregateException`.

All `IAggregateError` implementations should use the first element of `InnerErrors` as the value of `InnerError`, 
or `null` if InnerErrors is empty or `null`.

<div class="warning">
Unlike exceptions, the value of the <code>InnerErrors</code> can be <code>null</code>.
It means that the error is not aggregated, but <code>InnerError</code> may be not <code>null</code>.
To avoid handling it manually, use the <code>GetInnerErrors()</code> extension, 
which always returns a not <code>null</code> <code>IError</code> collection.
</div>

For manual aggregation, use the AggregateError implementation.

Usage example:

```csharp
IAggregateError aggError = new AggregateError(
    "Aggregate error",
    new MessageError("Inner error 0"),
    new MessageError("Inner error 1"),
    new MessageError("Inner error 2")
);

IError[] innerErrors = aggError.InnerErrors!.ToArray();
Assert.Equal("Aggregate error", aggError.Message);
Assert.Equal("Inner error 0", innerErrors[0].Message);
Assert.Equal("Inner error 1", innerErrors[1].Message);
Assert.Equal("Inner error 2", innerErrors[2].Message);
```

GetInnerErrors example:
```csharp
IError aggError = new AggregateError(
    "Aggregate error",
    new MessageError("Inner error 0"),
    new MessageError("Inner error 1"),
    new MessageError("Inner error 2")
);
IError notAggError = new MessageError("Not aggregate inner").Context("Not aggregate error");
IError innerEmptyError = new MessageError("Not inner errors");

Assert.Equal(3, aggError.GetInnerErrors().Count);
Assert.Equal(1, notAggError.GetInnerErrors().Count);
Assert.Equal(0, innerEmptyError.GetInnerErrors().Count);
```


