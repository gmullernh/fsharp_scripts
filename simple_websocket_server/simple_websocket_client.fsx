open System
open System.Net
open System.Net.Sockets
open System.Text

// Break exception to stop while loop on console
exception BreakException

let listenForNewMessages (stream:NetworkStream) =
    // Create Async Listener for new messages
    async {
        while true do
            let bufferContent:byte[] = Array.create 255 (new byte());
            let mutable data = "";
            while not (stream.Read(bufferContent, 0, bufferContent.Length).Equals(0)) do
                data <- System.Text.Encoding.UTF8.GetString(bufferContent)
                printfn "%s" data
    } |> Async.Start

let closeConnection (client:TcpClient) (networkStream:NetworkStream) =
    networkStream.Close()
    client.Close()

let sendMessage (message:string, netStream:NetworkStream) = 
    let data = System.Text.Encoding.UTF8.GetBytes(message:string)
    netStream.Write(data, 0, data.Length)
    printf "Message sent."

// [<EntryPoint>]
let main:int =
    let addr:string = IPAddress.Loopback.ToString()
    let port:int = 5050

    try
        printfn "Connecting to TCP Server at: %s:%i" addr port
        let client:TcpClient = new TcpClient(addr, port)
        let netStream:NetworkStream = client.GetStream()
        listenForNewMessages netStream

        printfn "Press ESCAPE key to exit"
        
        // Message Loop
        while true do
            sendMessage (Console.ReadLine(), netStream) 

        netStream.Close() 
        client.Close()
        0
    with
    | _ -> 0
    
    // Wait to quit
main