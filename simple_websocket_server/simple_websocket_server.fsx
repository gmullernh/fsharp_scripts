open System
open System.Net
open System.Net.Sockets
open System.Text

let addr:string = IPAddress.Any.ToString()
let port:int = 5050
let connectionLimit = 8
let mutable currentConnections = 0

// Creates an async HTTP Listener
let listen addr port =
    let tcpListener = new TcpListener(IPAddress.Parse(addr), port)
    tcpListener.Start()
    printfn "TCP Server listening at: %s:%i" addr port

    async {
        while true do
            while currentConnections < connectionLimit do
                let client = tcpListener.AcceptTcpClient()
                printf "Client %s connected" (client.Client.AddressFamily.ToString())
                currentConnections <- currentConnections + 1

                let bytes:byte[] = Array.create 255 (new byte());
                let mutable data = "";

                let networkStream = client.GetStream()

                while not (networkStream.Read(bytes, 0, bytes.Length).Equals(0)) do
                    data <- System.Text.Encoding.UTF8.GetString(bytes)
                    printfn "%s" (data)
        
                client.Close()
                currentConnections <- currentConnections - 1

    } |> Async.Start

// Listen for
listen addr port

// Wait to quit
printfn "Press any key to exit"
Console.ReadKey();