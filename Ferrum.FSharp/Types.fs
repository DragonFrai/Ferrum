namespace Ferrum.FSharp


type IError = Ferrum.IError
type IAggregateError = Ferrum.IAggregateError
type ITracedError = Ferrum.ITracedError
type Result<'a> = Result<'a, IError>
type IErrorFormatter = Ferrum.Formatting.IErrorFormatter
