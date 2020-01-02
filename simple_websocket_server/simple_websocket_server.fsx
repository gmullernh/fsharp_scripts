open System
open System.Net
open System.Net.Sockets
open System.Text

// Creates an async HTTP Listener
let listen addr port =
    let tcpListener = new TcpListener(IPAddress.Parse(addr), port)
    tcpListener.Start()
    printfn "TCP Server listening at: %s:%i" addr port

    async {
        while true do
            let client = tcpListener.AcceptTcpClient()
            printf "Client %s connected" (client.Client.LocalEndPoint.ToString())
            let bytes:byte[] = Array.create 255 (new byte());

            while not (client.GetStream().Read(bytes, 0, bytes.Length).Equals(0)) do
                Console.ForegroundColor <- ConsoleColor.Cyan;
                printfn "%s" (System.Text.Encoding.UTF8.GetString(bytes))

    } |> Async.Start

let main:int =
    let addr:string = IPAddress.Any.ToString()
    let port:int = 5050
    // Listen for
    listen addr port

    // Wait to quit
    printfn "Press any key to exit"
    Console.ReadKey() |> ignore
    0

main