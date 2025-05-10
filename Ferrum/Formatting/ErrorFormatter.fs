namespace Ferrum

open Ferrum


[<Interface>]
type IErrorFormatter =

    abstract Format: error: IError -> string
