# Error Formatting

Errors can implement `ToString` or `IFormattable` for formatting specific to them.
But to keep error formatting consistent, it is recommended to use error formatters 
(`IErrorFormatter` interface). 
In specific situations, this may reduce the amount of information about error, 
but allows errors to be formatted more predictable.

Default formatters is `MessageErrorFormatter`, `SummaryErrorFormatter`,
`DetailedErrorFormatter`, `DiagnosticErrorFormatter` and `NullErrorFormatter`.
All formatters have an `Instance` property with them instances.

You can also get the formatter by format string or verbosity level using the functions 
`ErrorFormatter.ByFormat(string)` and `ErrorFormatter.ByLevel(int)` respectively.
For each format string (except `NullErrorFormatter`) there is a corresponding extension method for 
the `IError` type, using which you can quickly format the error.

| Formatter                | Format   | Level | Method    | Messages | Lines  | Stack trace           |
|--------------------------|----------|-------|-----------|----------|--------|-----------------------|
| MessageErrorFormatter    | `M`/`L1` | 1     | `FormatM` | Final    | Single | No                    |
| SummaryErrorFormatter    | `S`/`L2` | 2     | `FormatS` | All      | Single | No                    |
| DetailedErrorFormatter   | `D`/`L3` | 3     | `FormatD` | All      | Multi  | For most depth error  |
| DiagnosticErrorFormatter | `X`/`L4` | 4     | `FormatX` | All      | Multi  | For all traced errors |
| NullErrorFormatter       | `N`/`L0` | 0     | -         | -        | -      | -                     |

So the error can be formatted in a variety of ways:

```csharp
SummaryErrorFormatter.Instance.Format(error);
error.Format(SummaryErrorFormatter.Instance);
ErrorFormatter.ByFormat("S").Format(error);
error.Format(ErrorFormatter.ByFormat("S"));
ErrorFormatter.ByLevel(2).Format(error);
error.Format(ErrorFormatter.ByLevel(2));
error.FormatS();
$"{error:S}"; // Specific for Ferrum buildin errors
```
All examples are equivalent (except string interpolation).
Hope everyone enjoys the factorial examples!

<div class="warning">
Errors provided by Ferrum are <code>IFormattable</code>, but other errors may not be. 
Therefore, it is recommended to explicitly use the formatters.
If you implement <code>IError</code>, you can inherit from the <code>BaseError</code> type if possible.
It provides <code>IFormattable</code> and <code>ToString</code> implementations for IError by default.
(Default formatter is <code>SummaryErrorFormatter</code>)
</div>

(In version v0.4 not implemented FormatProvider for errors. I don't even understand if it's necessary)


## Format result examples:

`MessageErrorFormatter`:

> ```User not created```

---

`SummaryErrorFormatter`:

> ```User not created: DB unreachable: I/O error```

---

`DetailedErrorFormatter` or `DiagnosticErrorFormatter` without tracing:

> ```
> [0] Error: User not created
> [1] Cause: DB unreachable
> [2] Cause: I/O error
> 
> ```

---

`DetailedErrorFormatter` with tracing:

> ```
> [0] Error: User not created
> [1] Cause: DB unreachable
> [2] Cause: I/O error
> Trace [1]:
>    at ...
>
> ```

---

`DiagnosticErrorFormatter` with tracing:

>```
> [0] Error: User not created
> [1] Cause: DB unreachable
> [2] Cause: I/O error
> Trace [0]:
>    at ...
> Trace [1]:
>    at ...
> 
> ```

---

Note that `DetailedErrorFormatter` and `DiagnosticErrorFormatter` end the formatted string with a line break.
