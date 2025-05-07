module Ferrum.Tests.Tests

open Xunit
open Ferrum
open Ferrum.Tests.Utils

[<Fact>]
let ``MessageError works`` () =
    let test (f: string -> IError) : unit =
        let err = f "msg error"
        Assert.Equal("msg error", err.Message)
        Assert.Equal(ValueNone, err.InnerError)
    do test (fun msg -> MessageError(msg))
    do test Error.anyhow
    do test (fun msg -> Result.anyhow msg |> Result.getError)

type ErrorToWrap = ErrorToWrap

[<Fact>]
let ``WrappedError works`` () =
    let test (f: ErrorToWrap -> IError) : unit =
        let errorToWrap = ErrorToWrap
        let err = f errorToWrap
        Assert.Equal(errorToWrap.ToString(), err.Message)
        Assert.Equal(ValueNone, err.InnerError)
    do test (fun err -> WrappedError(err))
    do test Error.wrap
    do test (fun err -> Result.wrap (Error err) |> Result.getError)

[<Fact>]
let ``ContextError works`` () =
    let test (f: string -> IError -> IError) : unit =
        let rootError = Error.anyhow "RootError"
        let err = f "Context" rootError
        Assert.Equal("Context", err.Message)
        Assert.Equal(ValueSome rootError, err.InnerError)
    do test (fun ctx err -> ContextError(ctx, err))
    do test Error.context
    do test (fun ctx err -> Result.context ctx (Error err) |> Result.getError)

[<Fact>]
let ``Error chain works`` () =
    let test (chain: IError -> IError seq) : unit =
        let error1 = Error.anyhow "Error1"
        let error2 = Error.context "Error2" error1
        let error3 = Error.context "Error3" error2
        Assert.Equal([error3; error2; error1], chain error3)
    do test (fun err -> err.Chain())
    do test Error.chain

[<Fact>]
let ``Error get root works`` () =
    let test (getRoot: IError -> IError) : unit =
        let error1 = Error.anyhow "Error1"
        let error2 = Error.context "Error2" error1
        let error3 = Error.context "Error3" error2
        Assert.Equal(error1, getRoot error3)
    do test (fun err -> err.GetRoot())
    do test Error.getRoot

[<Fact>]
let rec ``AggregateError with single aggregated error works`` () =
    let errors = [| Error.anyhow "err1" |]
    let agg = Error.aggregate "agg" errors
    let errors' = agg.Sources |> Seq.toArray
    Assert.Equal("agg", Error.message agg)
    Assert.Equal(ValueSome (errors[0]), Error.innerError agg)
    Assert.Equal<IError>(errors, errors')

[<Fact>]
let rec ``AggregateError contains original error sequence in original order`` () =
    let errors = [| "err1"; "err2"; |] |> Array.map Error.anyhow
    let agg = Error.aggregate "agg" errors
    let errors' = agg.Sources |> Seq.toArray
    Assert.Equal<IError>(errors, errors')

[<Fact>]
let rec ``AggregateError with empty errors suitable`` () =
    let errors = [| |]
    let agg = Error.aggregate "agg" errors
    Assert.True(agg.Sources |> Seq.isEmpty)
    Assert.Equal(ValueNone, Error.innerError agg)

[<Fact>]
let rec ``AggregateError Source member returns first error in aggregated sequence`` () =
    let err1 = Error.anyhow "err1"
    let err2 = Error.anyhow "err2"
    let agg = Error.aggregate "agg" [| err1; err2 |]
    Assert.Equal(ValueSome err1, Error.innerError agg)

