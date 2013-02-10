module Totify.Filters.Master

open TinyNLP
open TinyNLP.Tokenizer
open Totify.Protocol

// WARNING performance Trim added
let prepareText text =     
    let r = TinyNLP.Tokenizer.tokenize text
    match r with
        | None ->             
            [ {Id = 1; Content = "prepareText error"; Class = TokenClass.Other; Tag = ["UNKN"]; Stem = ""} ]
        | _ -> 
            let t = Totify.Utils.tagTokens (["<S>"] @ r.Value @ ["</S>"])
            r.Value |> List.mapi (fun i (x:string) -> 
                {
                Id = i; 
                Content = x.Trim();
                Stem = TinyNLP.Stemming.Stem x;
                Class = TinyNLP.Tokenizer.tokenClassifier x;
                Tag = t.[i]})


// FILTER #1: Synonymize It!
let synonymFilter tokensList = 
    tokensList |> List.map (fun x -> 
        { Token = x;
          Changes = [ { FilterName="synonyms"; Variants = TinyNLP.Synonymizer.getSynonyms(x.Stem) x.Tag.Head } ]
        })

// FILTER #2: Kill The Bad Words
let replaceFilter nodes =
    nodes |> List.map (fun x -> 
        let replaces = Filter2.searchReplaces x.Token.Content
        match replaces with 
            | None -> x
            | _ -> { Token = x.Token; Changes = x.Changes @ [{FilterName="replaces"; Variants = replaces.Value} ]})


let totify text =
    prepareText text |> synonymFilter |> replaceFilter