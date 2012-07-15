module Storage

open LanguageModel
open System.Collections.Generic
open ProtoBuf

let empty_probs = 
    {
    Unigrams =  new Dictionary<string, int>(); 
    Bigrams =  new Dictionary<string, int>(); 
    Trigrams =  new Dictionary<string, int>();
    Size = 0}

[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
type LanguageModel (model : Probs, indexed: Dictionary<string, string>) = class  
    member val Model : Probs = model with get, set
    member val Indexed : Dictionary<string, string> = indexed with get, set   
    new() = LanguageModel(empty_probs, new Dictionary<string, string>())
    override x.ToString() = x.Model.Unigrams.Count.ToString()
 end 