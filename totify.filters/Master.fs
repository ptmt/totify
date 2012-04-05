module Totify.Filters

open TinyNLP
open TinyNLP.Tokenizer
open Totify.Protocol

// WARNING performance Trim added
let prepareText text =     
    let r = TinyNLP.Tokenizer.tokenize text
    match r with
        | None -> [ {Id = 1; Content = "prepareText error"; Class = TokenClass.Other} ]
        | _ -> r.Value |> List.mapi (fun i x-> { Id = i; Content = x.Trim(); Class = TinyNLP.Tokenizer.tokenClassifier x})


// FILTER #1: Synonymize It!
let synonymFilter tokensList = 
    tokensList |> List.map (fun x -> { Token = x; Changes = [{ Variants = TinyNLP.Synonymizer.getSynonyms(TinyNLP.Stemming.Stem x.Content)} ]})


let totify text =
    prepareText text |> synonymFilter