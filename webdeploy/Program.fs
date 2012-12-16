module Totify.Web.Handler


open Mario.HttpContext
open Mario.WebServer



let myHandler (req:HttpRequest) : HttpResponse =
    match req.Uri with 
        | "/test.fs" -> 
            Totify.WebDeploy.MSBuild.test_build 
            { Json = "ok" }     
        | _ -> Mario.HttpUtility.badRequest
    

Mario.Start(myHandler, 8100)