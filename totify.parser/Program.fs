module Totify.Parser.TestRuntime

open Totify.Parser.LanguageModel
open Totify.Parser.Processing


//Totify.Parser.Processing.temporary_wrapper_parse (System.DateTime.Now.ToString()) |> printfn "%A"
//
////use f = System.IO.File.CreateText("c:\\data\\tfidf.txt")   
//
////search_2ch_model.ModelsByContexts    
////    |> Seq.iter (fun x -> x.Value.Unigrams |> Seq.take 10 |> Seq.iter (fun keyvalue -> printfn "%A" keyvalue))
//
//  
//
//  
let a = Simularity.tfidf_unigrams search_2ch_model
a |> printfn "%A"



//Simularity.sim_cosine "<S>" "</S>" a |> printfn "sim_cosine %A" 
let timer = new System.Diagnostics.Stopwatch()
timer.Start()
  
Simularity.find_sim a 
    |> printfn "%A"

timer.Stop()
printfn "find_sim in %i ms" timer.ElapsedMilliseconds
//Totify.Parser.Processing.temporary_wrapper_parse (System.DateTime.Now.ToString()) |> printfn "%A"