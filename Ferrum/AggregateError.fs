namespace Ferrum


[<Interface>]
type IAggregateError =
    inherit IError

    abstract Sources: IError seq
