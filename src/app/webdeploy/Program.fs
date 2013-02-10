module Totify.Web.Handler


open Mario.HttpContext
open Mario.WebServer



let myHandler (req:HttpRequest) : HttpResponse =
    match req.Uri with 
        | "/build.fs" ->             
            { Json = Totify.WebDeploy.totify_build  }             
        | _ -> Mario.HttpUtility.badRequest
    

Mario.Start(myHandler, 8100)