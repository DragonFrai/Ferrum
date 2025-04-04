namespace Ferrum


[<Interface>]
type IError =
    abstract Reason: string
    abstract Source: IError voption

[<RequireQualifiedAccess>]
module Error =

    let inline reason (err: IError) : string =
        err.Reason

    let inline source (err: IError) : IError voption =
        err.Source


type Result<'a> = Result<'a, IError>
