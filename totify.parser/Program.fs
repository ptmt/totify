module Totify.Parser

open LanguageModel
open Processing

for i in [1..5] do
    shannon_visualization_bigrams all_bigrams "<S>" "" |> printfn "%A"
  
let first_bigram =     
    let rand = new System.Random()
    let beginsents = all_trigrams |> List.filter (fun x-> x.Key.Split(' ').[0] = "<S>")     
    let tr = beginsents.[rand.Next(beginsents.Length)].Key
    tr.Split(' ').[0] + " " + tr.Split(' ').[1]

for i in [1..5] do
    shannon_visualization_trigrams all_trigrams first_bigram "" |> printfn "%A"