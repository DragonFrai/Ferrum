# IError Interface

`Ferrum.IError` is the interface of a basic error value, typically used in result types of the form 
`Result<TValue, TError>`. 

F# supports the `Result` type in its standard library, 
and C# has various community libraries such as DotNext (Their use is optional, but wrappers improve usability).

IError contains a string error message `IError.Message` (expected to be a single line, without trailing punctuation)
and can provide low-level error via the `IError.InnerError` property.
To display errors as string, see [Error Formatting](ErrorFormatting.md).
