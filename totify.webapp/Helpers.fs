module Totify.Web.Helpers

open System.Web.Script.Serialization
open Mario.HttpContext

let json t =
    let js = new JavaScriptSerializer()    
    js.Serialize(t)

let jsonp s = 
    "parse_totify_response(" + s + ");"

let badRequest = 
    {Json = "bad request"}