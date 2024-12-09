open System
open Utilities
open System.Windows.Forms
open UI

[<EntryPoint>]
let main argv =
    let form = CreateForm()
    try
        Application.Run(form)
    with
    | :? InvalidOperationException -> printfn "Invalid Operation"
    | (ex: exn) -> printfn "Exception occurred: %s" ex.Message
    0

