module Ferrum.Tests.Tests

open Xunit
open Ferrum
open Ferrum.Tests.Utils

[<Fact>]
let ``MessageError works`` () =
    let test (f: string -> IError) : unit =
        let err = f "msg error"
        Assert.Equal("msg error", err.Reason)
        Assert.Equal(ValueNone, err.Source)
    do test (fun msg -> MessageError(msg))
    do test Error.message
    do test (fun msg -> Result.message msg |> Result.getError)

type ErrorToWrap = ErrorToWrap

[<Fact>]
let ``WrappedError works`` () =
    let test (f: ErrorToWrap -> IError) : unit =
        let errorToWrap = ErrorToWrap
        let err = f errorToWrap
        Assert.Equal(errorToWrap.ToString(), err.Reason)
        Assert.Equal(ValueNone, err.Source)
    do test (fun err -> WrappedError(err))
    do test Error.wrap
    do test (fun err -> Result.wrap (Error err) |> Result.getError)

[<Fact>]
let ``ContextError works`` () =
    let test (f: string -> IError -> IError) : unit =
        let rootError = Error.message "RootError"
        let err = f "Context" rootError
        Assert.Equal("Context", err.Reason)
        Assert.Equal(ValueSome rootError, err.Source)
    do test (fun ctx err -> ContextError(ctx, err))
    do test Error.context
    do test (fun ctx err -> Result.context ctx (Error err) |> Result.getError)

[<Fact>]
let ``Error chain works`` () =
    let test (chain: IError -> IError seq) : unit =
        let error1 = Error.message "Error1"
        let error2 = Error.context "Error2" error1
        let error3 = Error.context "Error3" error2
        Assert.Equal([error3; error2; error1], chain error3)
    do test (fun err -> err.Chain())
    do test Error.chain

[<Fact>]
let ``Error get root works`` () =
    let test (getRoot: IError -> IError) : unit =
        let error1 = Error.message "Error1"
        let error2 = Error.context "Error2" error1
        let error3 = Error.context "Error3" error2
        Assert.Equal(error1, getRoot error3)
    do test (fun err -> err.GetRoot())
    do test Error.getRoot

[<Fact>]
let rec ``AggregateError with single aggregated error works`` () =
    let errors = [| Error.message "err1" |]
    let agg = Error.aggregate "agg" errors
    let errors' = AggregateError.errors agg |> Seq.toArray
    Assert.Equal("agg", Error.reason agg)
    Assert.Equal(ValueSome (errors[0]), Error.source agg)
    Assert.Equal<IError>(errors, errors')

[<Fact>]
let rec ``AggregateError contains original error sequence in original order`` () =
    let errors = [| "err1"; "err2"; |] |> Array.map Error.message
    let agg = Error.aggregate "agg" errors
    let errors' = AggregateError.errors agg |> Seq.toArray
    Assert.Equal<IError>(errors, errors')

[<Fact>]
let rec ``AggregateError with empty errors suitable`` () =
    let errors = [| |]
    let agg = Error.aggregate "agg" errors
    Assert.True(AggregateError.errors agg |> Seq.isEmpty)
    Assert.Equal(ValueNone, Error.source agg)

[<Fact>]
let rec ``AggregateError Source member returns first error in aggregated sequence`` () =
    let err1 = Error.message "err1"
    let err2 = Error.message "err2"
    let agg = Error.aggregate "agg" [| err1; err2 |]
    Assert.Equal(ValueSome err1, Error.source agg)

