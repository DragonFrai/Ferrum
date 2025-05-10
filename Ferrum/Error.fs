namespace Ferrum


[<Interface>]
type IError =

    /// <summary> </summary>
    /// <returns> Message string or null </returns>
    abstract Message: string

    abstract InnerError: IError voption
