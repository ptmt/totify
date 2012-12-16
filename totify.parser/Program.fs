#if INTERACTIVE
#time
#I @"D:\projects\totify\totify.parser\bin\Debug\"
#r "TinyNLP.fs.dll"
#r "totify.parser.exe"
#endif


open Totify.Parser.LanguageModel
open Totify.Parser.Processing
open System.Collections.Generic

//Totify.Parser.Processing.temporary_wrapper_parse (System.DateTime.Now.ToString()) |> printfn "%A"
//

//let f = System.IO.File.CreateText("c:\\data\\source.txt")   
//search_2ch_model.ModelsByContexts |> Seq.iter (fun x -> x.Value.Unigrams |> Seq.take 4 |> Seq.iter (fun keyvalue -> fprintf f "%A " keyvalue))

//printfn "unigrams size total = %A" (search_2ch_model.ModelsByContexts |> Seq.fold(fun acc (x:KeyValuePair<string, Probs>) -> acc + x.Value.Unigrams.Count) 0 )

let test_tokenize b =       
        TinyNLP.Tokenizer.splitSentences b
        |> List.ofArray 
        |> List.map (fun s -> 
            match (TinyNLP.Tokenizer.tokenize s) with
                    | Some tokens when tokens.Length > 0 -> ["<S>"] @ tokens @ ["</S>"] |> List.filter (fun t -> t.Trim() <> "") |> List.map (fun x-> x.ToLower())
                    | _ -> [])  

let test_models_by_context (langmodel:Totify.Parser.Storage.LanguageModel) =      
    [ "Вино это такой напиток";
     "Вода это такой вполне себе напиток";
     "Что такое вино? Это такой напиток, но не вода" ]    
    |> List.map test_tokenize    
    |> Seq.mapi (fun i x ->    
                    let context_key = "test" + string i
                    if langmodel.ModelsByContexts.ContainsKey(context_key) = false then
                        langmodel.ModelsByContexts.Add(context_key,  {
                                                    Unigrams =  new Dictionary<string, int>(); 
                                                    Bigrams =  new Dictionary<string, int>(); 
                                                    Trigrams =  new Dictionary<string, int>();
                                                    Size = 0})                    
                    (context_key, List.fold (fun aсс b -> handleSentence b aсс) langmodel.ModelsByContexts.[context_key] x ))

//let b = Simularity.tfidf_unigrams search_2ch_model

let a = 
    let m = new Totify.Parser.Storage.LanguageModel()
    test_models_by_context m
        |> Seq.iter (fun x -> match m.ModelsByContexts.ContainsKey(fst x) with
            | true ->  m.ModelsByContexts.[fst x] <- snd x
            | false -> m.ModelsByContexts.Add(x)
            )    
    m |> Simularity.tfidf_unigrams
//let print_a = 
//    a |> Seq.iter (fun (x:string*Dictionary<string, float>) -> (snd x) 
//                                                                |> Seq.sortBy (fun  (y:KeyValuePair<string, float>) -> - y.Value)                                                                
//                                                                |> Seq.take 1
//                                                                |> Seq.iter (fun (y:KeyValuePair<string, float>) -> printfn "%A" y)   )

                                                                
let k_limit = float 0.000001

let filter_token (token:string) = 
    let match_tokens = 
        ["<s>"; "</s>"]    
    if token.Length > 2 && List.tryFind (fun x -> x = token) match_tokens = None then 
        true
    else
        false

let uniq where = 
    where         
    |> Seq.fold (fun not_uniq x -> not_uniq @ (snd x |> List.ofSeq |> List.filter(fun (y:KeyValuePair<string, float>) -> y.Value > k_limit) |>  List.map (fun (y:KeyValuePair<string, float>) -> y.Key) )) []  
    |> Seq.distinct
    |> Seq.filter filter_token
    |> Array.ofSeq

let matrix (u:string[])  (process_elem) where =     
    let l = u.Length - 1
    let arr = Array2D.create l l ("", "", 0.0)
    let mutable debug_total_count = 0
    for i = 1 to l-1 do
     //   if (i + 1 < l) then
            let x = u.[i]            
            for j = i + 1 to l-1 do  
                let (sim_k:float, count) = process_elem x u.[j] where                              
                debug_total_count <- count + debug_total_count
                arr.[i,j] <- (x, u.[j], sim_k)
    arr

let process_temp x y where = 
    0.1

let matrix_dic (u:string[]) (process_elem) where =
    let d = new Dictionary<string, Dictionary<string, float>>()
    let len = u.Length - 1
    let mutable debug_total_count = 0
    for i = 0 to (len - 1) do
        let x = u.[i]
        let indic = new Dictionary<string, float>()
        for j = i + 1 to len do
            let (sim_k:float, count) = process_elem x u.[j] where
            debug_total_count <- count + debug_total_count
            if sim_k > 0.0 then
                indic.Add (u.[j], sim_k)
        d.Add(x, indic)    
    printfn "debug total count %A" debug_total_count    
    d

let getf (s:Dictionary<string, float>) word = 
    let res, w = s.TryGetValue(word)
    if (res = true) then
        w
    else
        0.0
    
let sim_cosine first second where = 
    
    let (a1, a2, a3, a4) =
             where |>
                Seq.fold (fun (top, bot1, bot2, count) x ->
                     let f = getf (snd x) first                    
                     let s = getf (snd x) second  
                     (top + f*s, bot1 + f*f, bot2 + s*s, count + 1)
                     )
                     (float 0, float 0, float 0, 0)
    ((sqrt (a1) / (sqrt a2 / sqrt a3)), a4)
    

let sim_jaccard first second where =   
        let getf (s:Dictionary<string, float>) word = 
            if s.ContainsKey(word) then
                s.[word]
            else
                float 0  
        let (min, max, count) =
             where 
                |> Seq.fold (fun (min, max, c) x ->                
                    let f = getf (snd x) first                    
                    let s = getf (snd x) second                                    
                    if f > s then (min + s, max + f, c + 1) else (min + f, max + s, c + 1)
                    ) (float 0, float 0, 0)

        //printfn "total count %A" count

        if max = float 0 then (float 0, count) else (min/max, count)

matrix_dic (uniq a) sim_jaccard a 

//let first_dic = a |> Seq.head |> snd
//
//let test j = 
//    for i = 1 to 100000 do
//         let a = getf first_dic ("привет" + string i) 
//         if (i % 1000 = 0) then 
//            printfn "%A" a



//let b = Simularity.find_sim a 
    
//b |> printfn "find_sim result = %A"

