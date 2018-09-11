// Learn more about F# at http://fsharp.org

open System
open Models
open System

let rec run () = 
    let input = Console.ReadLine()
    match input with
    | x when x = "" -> run()
    | _ -> ()

let persitSelicDataJob () =
    Console.WriteLine("starting")
    let selicOperationResult = SelicRate.tryGet()
    match selicOperationResult with
    | Success selicData -> 
            Console.WriteLine("got " + selicData.Length.ToString() + " items")
            ContentFilePersistence.persist selicData "TaxaSelic"
            Console.WriteLine("finishing")
    | Failure errors -> 
            Console.WriteLine(errors.[0])

[<EntryPoint>]
let main _ =
    printfn "Hello World from F#!"
    Scheduler.scheduleJob "0 0/1 * * * ?" persitSelicDataJob
    run()
    0 // return an integer exit code
