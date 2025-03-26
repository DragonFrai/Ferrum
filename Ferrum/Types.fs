namespace Ferrum

open System.Diagnostics


[<Interface>]
type IError =
    abstract Reason: string
    abstract Source: IError voption
    abstract StackTrace: string voption

[<Interface>]
type ILocalError =
    inherit IError
    abstract LocalStackTrace: StackTrace voption

[<RequireQualifiedAccess>]
module Error =

    let inline reason (err: IError) : string =
        err.Reason

    let inline source (err: IError) : IError voption =
        err.Source

    let inline stackTrace (err: IError) : string voption =
        err.StackTrace

    let inline localStackTrace (err: ILocalError) : StackTrace voption =
        err.LocalStackTrace


type Result<'a> = Result<'a, IError>
