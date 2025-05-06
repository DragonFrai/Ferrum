namespace Ferrum.Internal

open System.Collections
open System.Collections.Generic
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

    let tryFirst<'a> (xs: 'a seq) : 'a voption =
        match xs with
        | :? IReadOnlyList<'a> as xs ->
            if xs.Count > 0 then ValueSome xs[0] else ValueNone
        | :? IList<'a> as xs ->
            if xs.Count > 0 then ValueSome xs[0] else ValueNone
        | :? IList as xs ->
            if xs.Count > 0 then ValueSome (xs[0] :?> 'a) else ValueNone
        | errors ->
            use e = errors.GetEnumerator()
            if e.MoveNext() then ValueSome e.Current else ValueNone
