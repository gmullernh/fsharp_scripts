// A simple HTTP server for serving static HTML files and images for testing and development.
// To run it, use `fsi simple_http_server.fsx` from Powershell or Command Prompt

open System
open System.Net
open System.IO
open System.Text

// Map the content path to relative or absolute path based on the hostFolder 
let mapContentFolder folderPath:string =
    match folderPath with
    | folderPath when folderPath.ToString().StartsWith(@"\") -> Directory.GetCurrentDirectory().ToString() + folderPath.ToString()
    | _ -> folderPath.ToString()

// Creates an async HTTP Listener
let listener addr port (handler:(HttpListenerRequest->HttpListenerResponse->Async<unit>)) =
    let httpListener = new HttpListener()

    let formattedServer = sprintf "http://%s:%s/" addr (port.ToString()) // Add IP and Port
    httpListener.Prefixes.Add formattedServer 
    
    httpListener.Start()

    printfn "Server running at: %s" formattedServer

    let task = Async.FromBeginEnd(httpListener.BeginGetContext, httpListener.EndGetContext)
    async {
        while true do
            let! context = task
            Async.Start(handler context.Request context.Response)
    } |> Async.Start

// Fetch the response content as byte[] or Throw Not Found Exception
let byteResponse (req:HttpListenerRequest) =

    let file = sprintf "%s/%s" (mapContentFolder hostFolder) req.RawUrl
    printfn "Requested file : '%s'" file

    match file with 
    | file when File.Exists file -> File.ReadAllBytes(file)
    | _ -> raise (new FileNotFoundException())
   
// Returns the MIME Type for different contents
let mapMIMEType (req:HttpListenerRequest):string =
    let extension = Path.GetExtension(req.RawUrl)
    // Add new MIME Types here
    match extension with
    | ".jpg" -> "image/jpeg"
    | ".png" -> "image/png"
    | ".ico" -> "image/x-icon"
    | _ -> "text/html"

//[<EntryPoint>]
let main:int =
    // Server configurations
    let addr:string = IPAddress.Any.ToString()
    let port:int = 8088
    let hostFolder:string = @"\wwwroot"
    let message404:string = @"404 | File Not Found"

    // Encodes and returns the content
    listener addr port (fun req resp ->
        async {
            try
                resp.ContentType <- mapMIMEType req
                // if contains mime type of text, encodes in UTF-8
                let byteContent = byteResponse req
                resp.OutputStream.Write(byteContent, 0, byteContent.Length)
            with 
            | :? FileNotFoundException -> 
                let notFoundMessage = Encoding.UTF8.GetBytes(message404);
                resp.ContentType <- "text/html"
                resp.OutputStream.Write(notFoundMessage, 0, notFoundMessage.Length); |> ignore

            resp.OutputStream.Close()
        })

    // Wait to quit
    printfn "Press any key to exit"
    Console.ReadKey() |> ignore
    0

main