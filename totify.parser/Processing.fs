module Processing

open System.Collections.Generic
open LanguageModel

let split = char(' ')


let sentences_by_context = 
    WebGrabber.dvach_grabber 
    |> Seq.map (fun x ->  Extract.clean_html x |> Extract.clean_from_url) 
    |> Seq.filter (fun x -> x.Trim() <> "" && x <> "-" && x <> ">") 
    |> Seq.map (fun x -> TinyNLP.Tokenizer.splitSentences x
                                     |> List.ofArray |> List.map (fun s -> match (TinyNLP.Tokenizer.tokenize s) with
                                                                                    | Some tokens when tokens.Length > 0 -> ["<S>"] @ tokens @ ["</S>"] |> List.filter (fun t -> t.Trim() <> "") 
                                                                                    | _ -> []) )    
    
    
let models_by_context = sentences_by_context |> Seq.map (fun x -> x |> List.fold (fun aсс b -> handleSentence b aсс) Storage.empty_probs  )

let rec shannon_visualization_bigrams (bigrams:KeyValuePair<string, int> list) (word:string) (sentence:string) =
    let founded = bigrams  |> List.filter (fun x-> x.Key.Split(' ').[0] = word)     
    if founded.Length = 0 then
        "not found"
    else
        let i = new System.Random()
        let picked = founded.[i.Next(founded.Length)]
        if picked.Key.Contains("</S>") then
            sentence + " " + word + "</S>"
        else  
            shannon_visualization_bigrams bigrams (picked.Key.Split(' ').[1]) (sentence + " " + word)

let rec shannon_visualization_trigrams (trigrams:KeyValuePair<string, int> list) (bigram:string) (sentence:string) =
   
    let founded = trigrams  |> List.filter (fun x-> x.Key.Split(' ').[0] + " " + x.Key.Split(' ').[1] = bigram)     
    if founded.Length = 0 then
        "not found"
    else
        let i = new System.Random()
        let picked = founded.[i.Next(founded.Length)]
        if picked.Key.Contains("</S>") then
            sentence + " " + bigram.Split(' ').[1]  + "</S>"
        else
            let next_bigram = picked.Key.Split(' ').[1] + " " + picked.Key.Split(' ').[2]
            shannon_visualization_trigrams trigrams next_bigram (sentence + " " +  bigram.Split(' ').[1])
   

let all_bigrams_dict = 
    models_by_context |> Seq.fold (fun acc x -> TinyNLP.POST.Suffix.mergedict x.Bigrams acc) (new Dictionary<string, int>())
  

let all_bigrams =
    all_bigrams_dict |> Seq.filter (fun x-> x.Key <>"<S> </S>") |> Seq.sortBy (fun keyvalue -> keyvalue.Value) |> List.ofSeq |> List.rev


let all_trigrams = 
    models_by_context |> Seq.fold (fun acc x -> TinyNLP.POST.Suffix.mergedict x.Trigrams acc) (new Dictionary<string, int>())
    |> Seq.sortBy (fun keyvalue -> keyvalue.Value) |> List.ofSeq |> List.rev