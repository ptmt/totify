module Totify.Web.Handler


open Mario.HttpContext
open Mario.WebServer
open Totify.Filters.Master
open Totify.Web.Helpers



let myHandler (req:HttpRequest) : HttpResponse =
    match req.Uri with 
        | "/totify.fs" when req.Method = HttpMethod.POST -> { Json = (totify req.Body) |> json }
        | "/api.fs" when req.Method = HttpMethod.POST -> { Json = (totify req.Body) |> json |> jsonp }
        | "/panel/filter2.fs" -> Totify.Web.FiltersController.filter2Action req.Body req.Method    
        | "/panel/helper1.fs" -> Totify.Web.ServicesController.helper1Action req.Body req.Method    
        | _ -> { Json = "Unsupported request: " + req.Uri }
    

Mario.Start(myHandler)