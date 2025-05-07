namespace Ferrum.Examples


module Playground =

    [<EntryPoint>]
    let main argv =
        printfn "  === Custom errors example ==="
        do CustomErrors.run ()
        printfn "\n"

        printfn "  === Dynamic errors example ==="
        do DynamicErrors.run ()
        printfn "\n"

        printfn "  === Print greeting from file ==="
        do GreetingFromFile.run ()
        printfn "\n"

        printfn "  === Error formatting ==="
        do ErrorFormatting.run ()
        printfn "\n"

        0
