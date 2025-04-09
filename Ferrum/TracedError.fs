namespace Ferrum

open System.Diagnostics
open Ferrum


/// <summary>
/// Error that can contains stack trace.
/// </summary>
[<Interface>]
type ITracedError =
    inherit IError

    /// <summary>
    ///
    /// </summary>
    /// <remarks> Not null when LocalStackTrace not null </remarks>
    /// <returns> Stack trace string or null </returns>
    abstract StackTrace: string

    /// <summary>
    ///
    /// </summary>
    /// <returns> Stack trace string or null </returns>
    abstract LocalStackTrace: StackTrace
