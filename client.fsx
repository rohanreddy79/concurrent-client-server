open System
open System.Net
open System.Net.Sockets
open System.IO;
open System.Threading
open System.Threading.Tasks

let sendCommandsToServer (stream: NetworkStream,buffer:byte array) = 
    printf "Sending command: " 
    let inputFromUser = Console.ReadLine()
    stream.Write(System.Text.Encoding.ASCII.GetBytes(inputFromUser))
    let buff = Array.zeroCreate 256
    stream.Read(buff,0,256) |> ignore
    System.Text.Encoding.ASCII.GetString(buff)

let serverAddress = "127.0.0.1"
let serverPort = 9001

let client = new TcpClient(serverAddress,serverPort)
let mutable isClientUp = true
let buffer = Array.zeroCreate 256
let stream = client.GetStream()
let data = stream.Read(buffer,0,256)

let errors = Map ["-1","incorrect operation command";
                                     "-2","number of inputs is less than two";
                                      "-3","number of inputs is more than four";
                                      "-4","one or more of the inputs contain(s) non-number(s)";]

// Get Hello message from the server.
let message = System.Text.Encoding.ASCII.GetString(buffer)
printf "%s" message

// Continue processing when the client is up
while isClientUp do
    // Send the command to the sever from the console and return the response from the server
    let mutable response = sendCommandsToServer(stream,buffer)
    let keys = ["-1";"-2";"-3";"-4"]
    if response.Contains("-1") then
        response <- errors["-1"] 
    elif response.Contains("-2") then
        response <- errors["-2"]
    elif response.Contains("-3") then
        response <- errors["-3"]
    elif response.Contains("-4") then
        response <- errors["-4"]
    else
        if response.Contains("-5") then
            isClientUp <- false
    printfn "Server response: %s" response
printfn "exit"
client.Close()