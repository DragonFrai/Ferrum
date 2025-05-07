module [<AutoOpen>] Ferrum.ErrorExtensions

open System.Diagnostics


type IError with

    member inline this.Sources: IError seq =
        Error.sources this

    member inline this.Context(context: string) : IError =
        ContextError(context, this)

    [<StackTraceHidden>]
    member inline this.ContextTraced(context: string) : IError =
        ContextTracedError(context, this)

    member this.Chain() : IError seq =
        Error.chain this

    member this.GetRoot() : IError =
        Error.getRoot this

    member this.GetStackTrace() : string option =
        Error.stackTrace this

    member this.GetLocalStackTrace() : StackTrace option =
        Error.localStackTrace this

    member this.ToException() : ErrorException =
        ErrorException(this)

    member this.Raise<'a>() : 'a =
        raise (ErrorException(this))

    member inline this.Format(formatter: IErrorFormatter) : string =
        Error.format formatter this

    member inline this.Format(format: string) : string =
        Error.formatBy format this
