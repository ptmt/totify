module Totify.Filters.Filter2

open ProtoBuf
open System.Collections.Generic

// FILTER #2: REPLACE BAD WORDS

let empty_string = ""

[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
type ReplaceRule (find : string, replace : string) = class  
    member val Find : string = find with get, set   
    member val Replace : string = replace with get, set   
    new() = ReplaceRule(empty_string, empty_string)
    override x.ToString() = x.Find
end  

type ReplaceDict = Dictionary<string, string list>

let getAllReplaces =     
    Kevo.Store.findByQuery<ReplaceRule> (fun x-> true)
    
let searchReplaces to_find = 
    let initdict = 
        let dict = new Dictionary<string, string list>()
        Kevo.Store.findByQuery<ReplaceRule> (fun x-> true)
        |> List.iter (fun x ->
            if dict.ContainsKey(x.Find) = false then
                dict.Add(x.Find, [x.Replace])
            else
                dict.[x.Find] <- dict.[x.Find] @ [x.Replace]
            )
        dict
    let d = Kevo.Store.memo<ReplaceDict> (fun () -> initdict)   
    if d.ContainsKey(to_find) then 
        Some d.[to_find]
    else
        None
    //Kevo.Core.getDictionary<ReplaceRule>

let insertRule find_text replace_text = 
    let a = new ReplaceRule(find_text, replace_text)
    let newid = Kevo.Store.lastid<ReplaceRule> + 1
    Kevo.Store.append<ReplaceRule> (newid, a, None)
    Kevo.AppendLog.waitForIt<ReplaceRule> |> ignore
    Kevo.MemoryCache.clearCache Kevo.Core.cacheIndex<string>