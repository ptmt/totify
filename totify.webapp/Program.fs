module Totify.Web

open System.Web.Script.Serialization
open Mario.HttpContext
open Mario.WebServer


let processText t = 
    let js = new JavaScriptSerializer()
    js.Serialize(t)


let myHandler (req:HttpRequest) : HttpResponse =
    match req.Method with
        | HttpMethod.GET -> { Json = "Yet Another GET Request" }
        | HttpMethod.POST -> { Json = processText req.Body }
        | _ -> { Json = "Others" }

Mario.Start(myHandler)