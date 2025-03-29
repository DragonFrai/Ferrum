namespace Ferrum.Tests.Utils


[<RequireQualifiedAccess>]
module Result =

    let getError (res: Result<'a, 'e>) : 'e =
        match res with
        | Ok _ -> failwith "Result with Ok"
        | Error err -> err
