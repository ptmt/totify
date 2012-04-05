module Totify.Web

open System.Web.Script.Serialization
open Mario.HttpContext
open Mario.WebServer
open Totify.Filters


let processText (t:string) = 
    let js = new JavaScriptSerializer()    
    //printfn "%A" (System.Text.Encoding.UTF8.GetBytes t)
    js.Serialize(totify t)
    
    
let myHandler (req:HttpRequest) : HttpResponse =
    match req.Method with
        | HttpMethod.GET -> { Json = "200 OK" }
        | HttpMethod.POST -> { Json = (processText req.Body) }
        | _ -> { Json = "Unsupported request" }

Mario.Start(myHandler)