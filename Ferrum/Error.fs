namespace Ferrum


[<Interface>]
type IError =

    /// <summary> </summary>
    /// <returns> Reason string or null </returns>
    abstract Reason: string

    abstract Source: IError voption
