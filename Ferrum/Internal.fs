namespace Ferrum.Internal

open System.Diagnostics
open Ferrum


module internal Utils =

    let inline getIfTraced
            ([<InlineIfLambda>] getter: ITracedError -> 'a)
            (defaultValue: 'a)
            (error: IError)
            : 'a =
        match error with
        | :? ITracedError as error -> getter error
        | _ -> defaultValue

    let stackTraceChecked (error: IError) : string =
        getIfTraced (fun error -> error.StackTrace) null error

    let localStackTraceChecked (error: IError) : StackTrace =
        getIfTraced (fun error -> error.LocalStackTrace) null error
