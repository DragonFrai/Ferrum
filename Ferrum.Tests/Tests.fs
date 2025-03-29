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
    do test (fun msg -> Errors.Message(msg))
    do test Error.message
    do test (fun msg -> Result.message msg |> Result.getError)

type ErrorToWrap = ErrorToWrap

[<Fact>]
let ``WrapError works`` () =
    let test (f: ErrorToWrap -> IError) : unit =
        let errorToWrap = ErrorToWrap
        let err = f errorToWrap
        Assert.Equal(errorToWrap.ToString(), err.Reason)
        Assert.Equal(ValueNone, err.Source)
    do test (fun err -> Errors.Wrap(err))
    do test Error.wrap
    do test (fun err -> Result.wrap (Error err) |> Result.getError)

[<Fact>]
let ``ContextError works`` () =
    let test (f: string -> IError -> IError) : unit =
        let rootError = Error.message "RootError"
        let err = f "Context" rootError
        Assert.Equal("Context", err.Reason)
        Assert.Equal(ValueSome rootError, err.Source)
    do test (fun ctx err -> Errors.Context(ctx, err))
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

