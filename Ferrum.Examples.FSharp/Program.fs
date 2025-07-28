namespace Ferrum.Examples


module Playground =

    [<EntryPoint>]
    let main argv =

        printfn "===== Dynamic errors example   ====="
        do DynamicErrors.run ()
        printfn "\n"

        printfn "===== Print greeting from file ====="
        do GreetingFromFile.run ()
        printfn "\n"

        printfn "===== Error formatting         ====="
        do ErrorFormatting.run ()
        printfn "\n"

        printfn "===== Exception converting     ====="
        do ExceptionConverting.run ()
        printfn "\n"

        printfn "===== General boxing     ====="
        do GeneralWrapping.run ()
        printfn "\n"



        0
