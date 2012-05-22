module Totify.Filters.Filter2

open ProtoBuf

// FILTER #2: REPLACE BAD WORDS

let empty_string = ""

[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
type ReplaceRule (find : string, replace : string) = class  
    member val Find : string = find with get, set   
    member val Replace : string = replace with get, set   
    new() = ReplaceRule(empty_string, empty_string)
    override x.ToString() = x.Find
 end  


let getAllReplaces = 
    //Kevo.Store.findByQuery<ReplaceRule> (fun x-> true)
    Kevo.Core.getDictionary<ReplaceRule>

let insertRule find_text replace_text = 
    let a = new ReplaceRule(find_text, replace_text)
    let newid = Kevo.Store.lastid<ReplaceRule> + 1
    Kevo.Store.append<ReplaceRule> (newid, a, None)