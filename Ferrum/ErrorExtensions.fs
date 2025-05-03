module [<AutoOpen>] Ferrum.ErrorExtensions

open System.Diagnostics


type IError with

    member inline this.Sources: IError seq =
        Error.sources this

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


    member this.ToException() : ErrorException =
        ErrorException(this)

    member this.Throw<'a>() : 'a =
        raise (ErrorException(this))


    member inline this.Format(formatter: IErrorFormatter) : string =
        Error.format formatter this

    member inline this.FormatFinal() : string =
        Error.formatFinal this

    member inline this.FormatChain() : string =
        Error.formatChain this

    member inline this.FormatMultiline() : string =
        Error.formatMultiline this

    member inline this.FormatMultilineTrace() : string =
        Error.formatMultilineTrace this

    member inline this.FormatMultilineTraceAll() : string =
        Error.formatMultilineTraceAll this
