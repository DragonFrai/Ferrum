module [<AutoOpen>] Ferrum.ErrorExtensions

open System.Diagnostics
open Ferrum.Formatting


type IError with

    member inline this.Context(context: string) : IError =
        ContextError(context, this)

    [<StackTraceHidden>]
    member inline this.ContextT(context: string) : IError =
        ContextTracedError(context, this)

    member this.Chain() : IError seq =
        Error.chain this

    member this.GetRoot() : IError =
        Error.getRoot this

    member this.GetStackTrace() : string voption =
        Error.stackTrace this

    member this.GetLocalStackTrace() : StackTrace voption =
        Error.localStackTrace this



    member inline this.Format(formatter: IErrorFormatter) : string =
        formatter.Format(this)

    member inline this.FormatTop() : string =
        (FinalErrorFormatter.Instance :> IErrorFormatter).Format(this)

    member inline this.FormatChain() : string =
        (ChainErrorFormatter.Instance :> IErrorFormatter).Format(this)

    member inline this.FormatMultiline() : string =
        (MultilineErrorFormatter.Instance :> IErrorFormatter).Format(this)

    member inline this.FormatMultilineTrace() : string =
        (MultilineTraceErrorFormatter.Instance :> IErrorFormatter).Format(this)

    member inline this.FormatMultilineTraceAll() : string =
        (MultilineTraceAllErrorFormatter.Instance :> IErrorFormatter).Format(this)
