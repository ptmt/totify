module Totify.Web

open System.Web.Script.Serialization
open Mario.HttpContext
open Mario.WebServer
open Totify.Filters.Master


let json t =
    let js = new JavaScriptSerializer()    
    js.Serialize(t)

let processText (t:string) = 
    totify t |> json

let filter2Helper (s:string) = 
    let param = s.Split('&') |> Array.map (fun x -> x.Split('=').[1])
    Totify.Filters.Filter2.insertRule param.[0] param.[1]
    printfn "%A" param
    "true"
    
let myHandler (req:HttpRequest) : HttpResponse =
    match req.Uri with 
        | "totify.fs" when req.Method = HttpMethod.POST -> { Json = (processText req.Body) }
        | "/filter2/backend.fs" when req.Method = HttpMethod.GET -> { Json = json Totify.Filters.Filter2.getAllReplaces }
        | "/filter2/backend.fs" when req.Method = HttpMethod.PUT -> { Json = filter2Helper req.Body } //        
        | _ -> { Json = "Unsupported request: " + req.Uri }
    

Mario.Start(myHandler)