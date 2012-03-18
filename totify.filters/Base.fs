module Totify.Filters.Base

open TinyNLP

type Word = {
    Type : string;
    Word : string
    }

let prepareText text =     
    let r = TinyNLP.Tokenizer.tokenize text
    match r with
        | None -> None
        | _ -> r.Value |> List.map (fun x-> { Type = "Token";  Word = x ``})