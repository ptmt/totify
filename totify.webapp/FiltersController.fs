module Totify.Web.FiltersController

open Totify.Web.Helpers
open Mario.HttpContext


let filter2Helper (s:string) = 
    let param = s.Split('&') |> Array.map (fun x -> x.Split('=').[1])
    Totify.Filters.Filter2.insertRule param.[0] param.[1]
    printfn "%A" param
    "true"

       // | "/filter2/backend.fs" -> { Json = json Totify.Filters.Filter2.getAllReplaces }
       // | "/filter2/backend.fs" when req.Method = HttpMethod.PUT -> { Json = filter2Helper req.Body } //        
let filter2Action (body:string) (http_method:HttpMethod) = 
    match http_method with
        | HttpMethod.PUT -> { Json = filter2Helper (body |> Mario.HttpUtility.decode) }
        | HttpMethod.GET -> { Json = json Totify.Filters.Filter2.getAllReplaces }
        | _ -> badRequest
