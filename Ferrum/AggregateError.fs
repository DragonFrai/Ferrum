namespace Ferrum


[<Interface>]
type IAggregateError =
    inherit IError

    abstract Sources: IError seq


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AggregateError =

    let sources (error: IAggregateError) : IError seq =
        error.Sources
