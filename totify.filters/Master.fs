module Totify.Filters

open TinyNLP
open TinyNLP.Tokenizer
open Totify.Protocol

// WARNING performance Trim added
let prepareText text =     
    let r = TinyNLP.Tokenizer.tokenize text
    match r with
        | None ->             
            [ {Id = 1; Content = "prepareText error"; Class = TokenClass.Other; Tag = ["UNKN"]} ]
        | _ -> 
            let t = Totify.NLP.tagTokens (["<S>"] @ r.Value @ ["</S>"])
            r.Value |> List.mapi (fun i x-> { Id = i; Content = x.Trim(); Class = TinyNLP.Tokenizer.tokenClassifier x; Tag = t.[i]})


// FILTER #1: Synonymize It!
let synonymFilter tokensList = 
    tokensList |> List.map (fun x -> { Token = x; Changes = [{ Variants = TinyNLP.Synonymizer.getSynonyms(TinyNLP.Stemming.Stem x.Content)} ]})

//let tagger (nodeList: Node list) = 
//    nodeList |> List.map (fun x -> { Token = x; Changes = x.Changes @ 
let totify text =
    prepareText text |> synonymFilter