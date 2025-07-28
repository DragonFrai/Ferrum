# Ferrum for F#

The `Ferrum.FSharp` package provides `Error` and `Result` modules with more F#-ideomatic wrappers
of existing C# implementations. 
Nullable values are replaced to optional values.
The `Result` module contains additional utility functions for creating and converting values of 
the form `Result<'a, IError>`. It also contains the alias `type Result<'a> = Result<'a, IError>`.

| Ferrum                                                | Ferrum.FSharp                                          |
|-------------------------------------------------------|--------------------------------------------------------|
| `new MessageError(msg)`                               | `Error.message msg` / `Result.message msg`             |
| `new ContextError(msg, inner)` / `inner.Context(msg)` | `Error.context msg inner` / `Result.context msg inner` |
| `new AggregateError(msg, inners)`                     | `Error.aggregate msg` / `Result.aggregate`             |
| `AnyError.OfValue(value)`                             | `Error.box value` / `Result.boxError`                  |
| `error.Message`                                       | `Error.getMessage error`                               |
| `error.InnerError`                                    | `Error.getInnerError error`                            |
| `error.GetInnerErrors()`                              | `Error.getInnerErrors error`                           |
| `error.GetStackTrace()`                               | `Error.getStackTrace`                                  |
| `error.Chain()`                                       | `Error.chain`                                          |
| `error.GetRoot()`                                     | `Error.getRoot`                                        |
| `error.ToException()`                                 | `Error.toException`                                    |
| `exception.ToError()`                                 | `Error.ofException`                                    |
| `error.Format(formatter)`                             | `Error.format formatter error`                         |
| `error.Format(format)`                                | `Error.formatBy format error`                          |
| `error.Format(level)`                                 | `Error.formatL level error`                            |
| `error.FormatM()`                                     | `Error.formatM error`                                  |
| `error.FormatS()`                                     | `Error.formatS error`                                  |
| `error.FormatD()`                                     | `Error.formatD error`                                  |
| `error.FormatX()`                                     | `Error.formatX error`                                  |
| ...                                                   | ...                                                    |
