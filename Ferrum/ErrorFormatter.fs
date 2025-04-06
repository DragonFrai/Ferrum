namespace Ferrum

open Ferrum


[<Interface>]
type IErrorFormatter =

    abstract Format: error: IError -> string


[<RequireQualifiedAccess>]
module ErrorFormatter =

    let inline format (error: IError) (formatter: IErrorFormatter) : string =
        formatter.Format(error)
