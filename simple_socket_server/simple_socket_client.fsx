open System
open System.Net
open System.Net.Sockets
open System.Text

let listenForNewMessages (stream:NetworkStream) =
    // Create Async Listener for new messages
    async {
        while true do
            let bufferContent:byte[] = Array.create 255 (new byte());
            while not (stream.Read(bufferContent, 0, bufferContent.Length).Equals(0)) do
                Console.ForegroundColor <- ConsoleColor.Cyan;
                printfn "%s" (System.Text.Encoding.UTF8.GetString(bufferContent))
                Console.ForegroundColor <- ConsoleColor.White;
    } |> Async.Start

let sendMessage (message:string, netStream:NetworkStream) = 
    let data = System.Text.Encoding.UTF8.GetBytes(message:string)
    netStream.Write(data, 0, data.Length)

// [<EntryPoint>]
let main:int =
    let addr:string = IPAddress.Loopback.ToString()
    let port:int = 5050
    let mutable continueToLoop = true;

    printfn "Connecting to TCP Server at: %s:%i" addr port
    let client:TcpClient = new TcpClient(addr, port)
    let netStream:NetworkStream = client.GetStream()
    listenForNewMessages netStream
    printfn @"Type \q key to exit"
    
    // Message Loop
    while continueToLoop do
        match Console.ReadLine() with
        | @"\q" -> continueToLoop <- false;
        | "" -> ()
        | _ -> sendMessage (Console.ReadLine(), netStream)

    netStream.Close() 
    client.Close()
    printfn "Shutting down the  TCP Server at: %s:%i" addr port
    0

main