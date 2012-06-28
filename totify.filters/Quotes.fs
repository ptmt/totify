module Totify.Filters.Quotes

// QOUTES AND RULES FOR RUSSIAN LANGUAGE

let empty_string = ""

open ProtoBuf
open System.Collections.Generic

[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
type QuoteItem (quote : string, author : string) = class  
    member val Author : string = author with get, set   
    member val Quote : string = quote with get, set   
    new() = QuoteItem(empty_string, empty_string)
    override x.ToString() = x.Quote
end  

let getAllQuotes =     
    match Kevo.Store.size<QuoteItem> with
    | 0 -> []
    | _ -> Kevo.Store.findByQuery<QuoteItem> (fun x-> true)
    
let insertQuote qoute author = 
    let a = new QuoteItem(qoute, author)
    let newid = Kevo.Store.lastid<QuoteItem> + 1
    Kevo.Store.append<QuoteItem> (newid, a, None)
    Kevo.AppendLog.waitForIt<QuoteItem> |> ignore
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<string>

let getRandomQuote =    
    let size = Kevo.Store.size<QuoteItem>
    if size > 0 then 
        let r = new System.Random()
        let index = r.Next(size)        
        Some getAllQuotes.[index]
    else
        None
    //let allqgetAllQuotes |> 