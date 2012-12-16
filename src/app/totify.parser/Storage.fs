module Totify.Parser.Storage

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
type LanguageModel (name: string, model : Dictionary<string,Probs>, indexed: List<string>, updatedat: string) = class  
    member val Name : string = name with get, set
    member val ModelsByContexts : Dictionary<string,Probs> = model with get, set
    member val Indexed : List<string> = indexed with get, set //Dictionary<string, string> = indexed with get, set   
    member val UpdatedAt : string = updatedat with get, set
    new() = LanguageModel("", new Dictionary<string,Probs>(), new List<string>(), "")
    override x.ToString() = x.Name + " with models count: " + x.ModelsByContexts.Count.ToString()
 end 

[<ProtoContract(ImplicitFields = ImplicitFields.AllPublic)>]
type SimularityExtract (name: string, matrix : Dictionary<string, List<(string * float)>>) = class  
    member val Name : string = name with get, set
    member val MatrixU : Dictionary<string, List<(string * float)>> = matrix with get, set
    member val MatrixB : Dictionary<string, List<(string * float)>> = matrix with get, set
    new() = SimularityExtract("", new Dictionary<string, List<(string * float)>>() )
    override x.ToString() = x.Name 
 end 