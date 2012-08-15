module Simularity

open Totify.Parser.Storage
open System.Collections.Generic
open Totify.Parser.LanguageModel

let tfidf_unigrams (m:LanguageModel) = 
           
    let d = m.ModelsByContexts.Count

    let count_in_docs unigram = 
        (m.ModelsByContexts
        |> Seq.filter (fun x -> x.Value.Unigrams.ContainsKey(unigram))
        |> Seq.length)

    let calc u f s = 
        (float f / float s)// * System.Math.Log(float d / float (count_in_docs u)))

    
    m.ModelsByContexts
        |> Seq.map (fun (m:KeyValuePair<string, Probs>) -> 
                let dic = new Dictionary<string, float>()
                m.Value.Unigrams |> Seq.iter (fun unigram -> dic.Add (unigram.Key, (calc (unigram.Key) unigram.Value m.Value.Size)))
                (m.Key, dic)
            )



let sim_cosine first second where = 
    let sum_by_context f m = 
        let sum_context context = 
            (snd context) 
            |> Seq.filter f
            |> Seq.map (fun (x:KeyValuePair<string,float>) -> m x.Value) 
            |> Seq.fold (fun acc y -> acc * y) (float 1)

        where |> Seq.fold (fun a x -> a + sum_context x) (float 0)

    sum_by_context (fun x -> x.Key = first || x.Key = second) (fun x-> x) / sqrt (sum_by_context (fun x -> x.Key = first) (fun x-> x * x)  * sum_by_context (fun b -> b.Key = second) (fun x-> x * x) )

let sim_jaccard first second where =   
    
     //printfn "%A %A" first second  
        let getf (s:Dictionary<string, float>) word = 
            if s.ContainsKey(word) then
                s.[word]
            else
                float 0  
        let (min, max) =
             where 
                |> Seq.fold (fun (min, max) x -> 
                    let f = getf (snd x) first
                    //printfn "%A %A" f first
                    let s = getf (snd x) second
                   // printfn "%A %A" s second
                    if f > s then (min + s, max + f) else (min + f, max + s)
                    ) (float 0, float 0)
        
        if min = max || max = float 0 then float 0 else min/max

let filter_token (token:string) = 
    let match_tokens = 
        ["<s>"; "</s>"]    
    if token.Length > 2 && List.tryFind (fun x -> x = token) match_tokens = None then 
        true
    else
        false

let find_sim where = 
    
    let k_limit = float 0.0005

    let uniq = 
       where         
        |> Seq.fold (fun not_uniq x -> not_uniq @ (snd x |> List.ofSeq |> List.filter(fun (y:KeyValuePair<string, float>) -> y.Value > k_limit) |>  List.map (fun (y:KeyValuePair<string, float>) -> y.Key) )) []  
        |> Seq.distinct
        |> Seq.filter filter_token
        |> Array.ofSeq
    
    //let matrix = uniq |> Seq.fold (fun matrix_list x -> Array.append matrix_list (uniq |> Array.ofSeq |> Array.map (fun y -> (x, y) ))  ) [||]
    
    let matrix = 
        let l = uniq.Length - 1
        seq {
            for i in [0..l] do
                if (i + 1 < l) then
                    for j in [i+1..l] do                                
                        yield (uniq.[i], uniq.[j])
        }


   // matrix 
   
   
    matrix 
   // |> Seq.take 10000
     |> Seq.map (fun elem -> (fst elem, snd elem, (sim_cosine (fst elem) (snd elem) where)) ) 
 //   |> Seq.map (fun elem -> async { return (fst elem, snd elem, (sim_jaccard (fst elem) (snd elem) where))  }) 
  //  |> Async.Parallel |> Async.RunSynchronously  
    |> Seq.sortBy (fun (x, y, z) -> z) 
    //|> Seq.take 1000
    |> List.ofSeq
    |> List.rev