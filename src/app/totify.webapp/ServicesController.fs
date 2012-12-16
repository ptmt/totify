module Totify.Web.ServicesController

open Totify.Web.Helpers
open Mario.HttpContext


let helper1Helper (s:string) = 
    let param = s.Split('&') |> Array.map (fun x -> x.Split('=').[1])
    Totify.Filters.Quotes.insertQuote param.[0] param.[1]
    printfn "%A" param
    "true"

let quoteHelper = 
    match Totify.Filters.Quotes.getRandomQuote with
        | None -> ""
        | Some x -> json x

let helper1Action 
    (body:string) (http_method:HttpMethod) = 
    match http_method with
        | HttpMethod.PUT -> { Json = helper1Helper (body |> Mario.HttpUtility.decode)}
        | HttpMethod.GET -> { Json = json Totify.Filters.Quotes.getAllQuotes }
        | _ -> Mario.HttpUtility.badRequest

let qouteAction 
    (body:string) (http_method:HttpMethod) = 
    match http_method with       
        | HttpMethod.GET -> { Json =  quoteHelper }
        | _ -> Mario.HttpUtility.badRequest