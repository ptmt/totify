module Totify.Filters

open TinyNLP
open Totify.Protocol


let prepareText text =     
    let r = TinyNLP.Tokenizer.tokenize text
    match r with
        | None -> [ {Id = 1; Content = "error"} ]
        | _ -> r.Value |> List.mapi (fun i x-> { Id = i; Content = x})


let synonymFilter tokensList = 
    tokensList |> List.map (fun x -> { Token = x; Changes = [{ Variants = TinyNLP.Synonymizer.getSynonyms(TinyNLP.Stemming.Stem x.Content)} ]})


let totify text =
    prepareText text |> synonymFilter