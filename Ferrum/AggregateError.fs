namespace Ferrum


[<Interface>]
type IAggregateError =
    inherit IError

    /// <summary>
    /// Indicates that this error is semantically aggregated.
    /// </summary>
    /// <remarks>
    /// For type based aggregated errors this flag is always true (as <see cref="AggregateError"/>),
    /// but for dynamically aggregated errors, this flag can be false if error constructed as not aggregated.
    /// It is useful for more relevant displaying errors
    /// (this flag can determine, serialize error with single source error to list or not).
    /// <br/>
    /// Can be false only when Sources contains single error.
    /// </remarks>
    abstract IsAggregate: bool

    abstract InnerErrors: IError seq
