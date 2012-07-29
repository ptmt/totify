module Totify.Parser.Extract

open System.Text.RegularExpressions

let clean_html raw_html = 
    let reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
    System.Web.HttpUtility.HtmlDecode(reg.Replace(raw_html, " "))
   
let clean_from_url text = 
    let reg = new Regex("((mailto\:|(news|(ht|f)tp(s?))\://){1}\S+)")     
    (reg.Replace(text, " "))