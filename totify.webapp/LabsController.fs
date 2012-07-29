module Totify.Web.LabsController

open Totify.Web.Helpers
open Mario.HttpContext


let Parse2chAction 
    (body:string) (http_method:HttpMethod) = 
    match http_method with     
        | HttpMethod.GET -> { Json = json (Totify.Parser.Processing.temporary_wrapper_parse (System.DateTime.Now.ToString()))}
        | _ -> badRequest

let Shannon2chAction 
    (body:string) (http_method:HttpMethod) = 
    let rand = new System.Random(System.DateTime.Now.Second)
    match http_method with      
        | HttpMethod.GET -> { Json = json (Totify.Parser.Processing.temporary_wrapper_shannon rand) }
        | _ -> badRequest

let StatAction 
    (body:string) (http_method:HttpMethod) = 
    { Json = json Totify.Parser.Processing.get_stat }