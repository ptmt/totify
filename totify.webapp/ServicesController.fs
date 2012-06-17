module Totify.Web.ServicesController

open Totify.Web.Helpers
open Mario.HttpContext


let helper1Helper (s:string) = 
    let param = s.Split('&') |> Array.map (fun x -> x.Split('=').[1])
    Totify.Filters.Quotes.insertQuote param.[0] param.[1]
    printfn "%A" param
    "true"

let helper1Action 
    (body:string) (http_method:HttpMethod) = 
    match http_method with
        | HttpMethod.PUT -> { Json = helper1Helper body }
        | HttpMethod.GET -> { Json = json Totify.Filters.Quotes.getAllQuotes }
        | _ -> badRequest