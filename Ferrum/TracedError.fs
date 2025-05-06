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
    /// Stack trace string with trailing newline or null
    /// </summary>
    /// <remarks>
    /// Not expected to be a stack trace, but it is recommended to follow
    /// the <see cref="System.Diagnostics.StackTrace.ToString()"/> format.
    /// </remarks>
    abstract StackTrace: string

    /// <summary>
    /// Local stack trace object or null
    /// </summary>
    abstract LocalStackTrace: StackTrace
