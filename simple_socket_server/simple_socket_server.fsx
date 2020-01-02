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
            printfn "Client %s connected" (client.Client.LocalEndPoint.ToString())

            let buffer = Array.zeroCreate<byte> 4096
            
            while not (client.GetStream().Read(buffer, 0, buffer.Length).Equals(0)) do
                let data = System.Text.Encoding.UTF8.GetString(buffer)
                Console.ForegroundColor <- ConsoleColor.Cyan

                if data.Trim().Length > 0 then printfn "%s" (data.Trim())

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