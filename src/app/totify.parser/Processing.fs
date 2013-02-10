module Totify.Parser.Processing

open System.Collections.Generic
open Totify.Parser.LanguageModel

let split = char(' ')

// ищем модель с name == 2ch, запоминаем её индекс, используем эту модель как начальную для фолдинга
// получаем индексированные доски с их тредами - передаем их в процедуру

let search_2ch_model = 
    let l = Kevo.Store.findByQuery<Storage.LanguageModel> (fun (x:Storage.LanguageModel) -> x.Name = "2ch")
    //let m = 
    if l.Length > 0 then
        l.[0]
    else
        new Storage.LanguageModel("2ch",  new Dictionary<string, Probs>(), new List<string>(), (System.DateTime.Now.ToLongDateString()))   // m
        

// возвращается набор предложений по контекстам - т.е. по доскам, добавляем их в индекс и делаем handleSentence, а затем сохраняем в index
// string list -> seq<(string * string list) * string list list>
let parse_sentences_by_context indexed_threads = 
   
    let tokenize b = 
        //printfn "%A" b
        TinyNLP.Tokenizer.splitSentences b
        |> List.ofArray 
        |> List.map (fun s -> 
                        match (TinyNLP.Tokenizer.tokenize s) with
                                | Some tokens when tokens.Length > 0 -> ["<S>"] @ tokens @ ["</S>"] |> List.filter (fun t -> t.Trim() <> "") |> List.map (fun x-> x.ToLower())
                                | _ -> [])    

    WebGrabber.dvach_grabber indexed_threads  
        |> Seq.map (fun x ->  ((fst x, fst (snd x)), Extract.clean_html (snd (snd x)) |> Extract.clean_from_url) ) 
        |> Seq.filter (fun (y, x) -> x.Trim() <> "" && x <> "-" && x <> ">")
        |> Seq.map (fun x -> (fst x, snd x |> tokenize))
    
    

let models_by_context sentences (langmodel:Storage.LanguageModel) =      
    sentences  
    |> Seq.map (fun x ->    
                    let context_key = fst (fst x)
                    if langmodel.ModelsByContexts.ContainsKey(context_key) = false then
                        langmodel.ModelsByContexts.Add(context_key,  {
                                                    Unigrams =  new Dictionary<string, int>(); 
                                                    Bigrams =  new Dictionary<string, int>(); 
                                                    Trigrams =  new Dictionary<string, int>();
                                                    Size = 0})
                    printfn "%A %A" (fst (fst x)) (snd x)
                    (context_key, List.fold (fun aсс b -> handleSentence b aсс) langmodel.ModelsByContexts.[context_key] (snd x) ))

let rec shannon_visualization_bigrams (bigrams:KeyValuePair<string, int> list) (word:string) (sentence:string) (rand:System.Random) =
    let founded = bigrams  |> List.filter (fun x-> x.Key.Split(' ').[0] = word)     
    if founded.Length = 0 then
        "not found"
    else      
        let picked = founded.[rand.Next(founded.Length)]
        if picked.Key.Contains("</s>") then
            sentence + " " + word + "</s>"
        else  
            shannon_visualization_bigrams bigrams (picked.Key.Split(' ').[1]) (sentence + " " + word) rand

let rec shannon_visualization_trigrams (trigrams:KeyValuePair<string, int> list) (bigram:string) (sentence:string) (rand:System.Random) =
   
    let founded = trigrams  |> List.filter (fun x-> x.Key.Split(' ').[0] + " " + x.Key.Split(' ').[1] = bigram)     
    if founded.Length = 0 then
        "not found"
    else
       
        let j = rand.Next(founded.Length)
       // printfn "founded = %A, j = %A" founded.Length j
        let picked = founded.[j]
        if picked.Key.Contains("</s>") then
            sentence + " " + bigram.Split(' ').[1]  + "</s>"
        else
            let next_bigram = picked.Key.Split(' ').[1] + " " + picked.Key.Split(' ').[2]
            shannon_visualization_trigrams trigrams next_bigram (sentence + " " +  bigram.Split(' ').[1]) rand


type Stat = {
    TotalIndexed : int;
    TotalUnigrams : int;
    TotalBigrams: int;
    TotalTrigrams: int;
    UpdatedAt: string;
    Now:string;
    }

let get_stat  =
    let current_model = search_2ch_model
    {
        TotalIndexed = current_model.Indexed.Count;
        TotalUnigrams = (current_model.ModelsByContexts |> Seq.fold (fun acc x -> acc + x.Value.Unigrams.Count ) 0);
        TotalBigrams = (current_model.ModelsByContexts |> Seq.fold (fun acc x -> acc + x.Value.Bigrams.Count ) 0);
        TotalTrigrams = (current_model.ModelsByContexts |> Seq.fold (fun acc x -> acc + x.Value.Trigrams.Count ) 0);
        UpdatedAt = current_model.UpdatedAt;
        Now = (System.DateTime.Now.ToShortDateString());
    }


 
let temporary_wrapper_parse (datetime:string) =  
    let current_model = search_2ch_model
    let sentences = parse_sentences_by_context (current_model.Indexed |> List.ofSeq)
    //sentences |> Seq.iter (fun x -> printfn "%A %A" (fst (fst x)) (snd x))
    sentences |> Seq.iter (fun x-> (snd (fst x)) |> Seq.iter (fun y -> if current_model.Indexed.Contains(y) = false then current_model.Indexed.Add(y)) ) 
    models_by_context sentences current_model 
        |> Seq.iter (fun x -> match current_model.ModelsByContexts.ContainsKey(fst x) with
                                | true ->  current_model.ModelsByContexts.[fst x] <- snd x
                                | false -> current_model.ModelsByContexts.Add(x)
            )
    current_model.UpdatedAt <- datetime
    Kevo.Store.update<Storage.LanguageModel> (0, current_model)
    Kevo.AppendLog.waitForIt<Storage.LanguageModel> |> ignore
    get_stat

type ShannonOut = 
    {
        ByTrigrams: string;
        ByBigrams: string;
    }

let temporary_wrapper_shannon rand = 
    let current_model = search_2ch_model
    let all_trigrams = 
        current_model.ModelsByContexts |> Seq.fold (fun acc x -> TinyNLP.POST.Suffix.mergedict x.Value.Trigrams acc) (new Dictionary<string, int>())
        |> Seq.sortBy (fun keyvalue -> keyvalue.Value) |> List.ofSeq |> List.rev

    let all_bigrams_dict = 
        current_model.ModelsByContexts |> Seq.fold (fun acc x -> TinyNLP.POST.Suffix.mergedict x.Value.Bigrams acc) (new Dictionary<string, int>())  

    let all_bigrams =
        all_bigrams_dict |> Seq.filter (fun x-> x.Key <> "<S> </S>") |> Seq.sortBy (fun keyvalue -> keyvalue.Value) |> List.ofSeq |> List.rev

    let first_bigram (rand:System.Random) =     

        let beginsents = all_trigrams |> List.filter (fun x-> x.Key.Split(' ').[0] = "<s>")       
        let i = rand.Next(beginsents.Length)      
       // printfn "i = %A" i
        if i < beginsents.Length then 
            let tr = beginsents.[i].Key
            tr.Split(' ').[0] + " " + tr.Split(' ').[1]
        else
            ""

    if current_model.Indexed.Count > 0 then
        let output = shannon_visualization_trigrams all_trigrams (first_bigram rand) "" rand        
        let output2 = shannon_visualization_bigrams all_bigrams "<s>" "" rand
        
        {ByTrigrams = output.Replace("<s>", "").Replace("</s>","").Replace(" ,",","); ByBigrams = output2.Replace("<s>", "").Replace("</s>","").Replace(" ,",",")}
    else
        {ByTrigrams = ""; ByBigrams = ""}
