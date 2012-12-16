#if INTERACTIVE
#time
#I @"D:\projects\totify\totify.parser\bin\Debug\"
#r "TinyNLP.fs.dll"
#r "totify.parser.exe"
#endif


open Totify.Parser.LanguageModel
open Totify.Parser.Processing
open System.Collections.Generic
open Totify.Parser.Storage

//Totify.Parser.Processing.temporary_wrapper_parse (System.DateTime.Now.ToString()) |> printfn "%A"
//

//let f = System.IO.File.CreateText("c:\\data\\source.txt")   
//search_2ch_model.ModelsByContexts |> Seq.iter (fun x -> x.Value.Unigrams |> Seq.take 4 |> Seq.iter (fun keyvalue -> fprintf f "%A " keyvalue))

//printfn "unigrams size total = %A" (search_2ch_model.ModelsByContexts |> Seq.fold(fun acc (x:KeyValuePair<string, Probs>) -> acc + x.Value.Unigrams.Count) 0 )

//let test_tokenize b =       
//        TinyNLP.Tokenizer.splitSentences b
//        |> List.ofArray 
//        |> List.map (fun s -> 
//            match (TinyNLP.Tokenizer.tokenize s) with
//                    | Some tokens when tokens.Length > 0 -> ["<S>"] @ tokens @ ["</S>"] |> List.filter (fun t -> t.Trim() <> "") |> List.map (fun x-> x.ToLower())
//                    | _ -> [])  
//
//let test_models_by_context (langmodel:Totify.Parser.Storage.LanguageModel) =      
//    [ @"Лепра, сегодня самый странный день в моей жизни. Наверное, стоит бухнуть и отметить жизнь как таковую. 
//Сегодня за стенкой кабинета у меня в офисе убито три человека. Двое из них работают совсем недавно на том месте. Когда я выбежал на лестницу, то там валялся еще мужик практически без головы, вокруг все в мозгах — это был день его трудоустройства собственно. Всего известно о пяти убитых и о нескольких человек различной степени тяжести. Сам я был в одном кабинете от смерти. 
//Я слышал хлопки и крики, через минуту к нам ворвались безопасники с оружием и выбежали, еще в течение 10 минут прилетел вертолет, приехали скорые и полиция.
//Парень был в полной амуниции с дробовиком, сайгой и весь напичкан обоймами и патронами. 
//Живите и радуйтесь жизни. Мир вам и любовь.";
//     @"Я понимаю, что три поста за день на тему последней перестрелки — это много, но бля. Почитал комментарии, там везде фоточки Брейвика, заплюсованные комменты с посылом, мол, вот Брейвик молодец, а наш какое–то чмо. В самом заплюсованном комментарии Брейвика назвали святым. Хотелось бы заострить именно здесь внимание. Совершенно непонятно, чем же один задрот, решивший сыграть в ГТА в реале, так принципиально отличается от второго?";
//     @"И именно сейчас при Вашем участии или неучастии во власть рвется виртуал неизвестно кого, чей лепроотец не появлялся последние два года, а чьи лепродети, пришедшие к нам по халявным инвайтам, со «смешными никами» никак себя не проявляют и скорее всего тоже виртуалы. Вы думаете, мне очень нужен пост ПГ? Вовсе нет. Сидеть целыми днями и удалять тупак, править орфографические ошибки, перезаливать картинки с имгуров и общаться в инбоксах с людьми, которые не удосужились хотя бы раз заглянуть в «Конституцию» — это сомнительное удовольствие. Но терпеть целую неделю анархии, ради пары часов смеха после избрания непонятно кого – это тоже перебор. Не хотите выбирать меня – выберете нынешнего ПГ, Йована, да хоть Соляриза! Только не надо унижать наш дорогой и любимый ресурс, выбирая ебаное ничего. Ебаное ничего – это и есть ебаное ничего. Шесть дней оголтелого тупака – это слишком много. В этом говне утонут все хорошие посты, захлебнется все то, что мы так уважаем и любим." ]    
//    |> List.map test_tokenize    
//    |> Seq.mapi (fun i x ->    
//                    let context_key = "test" + string i
//                    if langmodel.ModelsByContexts.ContainsKey(context_key) = false then
//                        langmodel.ModelsByContexts.Add(context_key,  {
//                                                    Unigrams =  new Dictionary<string, int>(); 
//                                                    Bigrams =  new Dictionary<string, int>(); 
//                                                    Trigrams =  new Dictionary<string, int>();
//                                                    Size = 0})                    
//                    (context_key, List.fold (fun aсс b -> handleSentence b aсс) langmodel.ModelsByContexts.[context_key] x ))

let a = Simularity.tfidf_unigrams search_2ch_model

//let b = 
//    let m = new Totify.Parser.Storage.LanguageModel()
//    test_models_by_context m
//        |> Seq.iter (fun x -> match m.ModelsByContexts.ContainsKey(fst x) with
//            | true ->  m.ModelsByContexts.[fst x] <- snd x
//            | false -> m.ModelsByContexts.Add(x)
//            )    
//    m |> Simularity.tfidf_unigrams


                                                                
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
    let l = 1000 // u.Length - 1
    let dic = new Dictionary<string, List<(string*float)>>()

    let mutable debug_total_count = 0
    for i = 1 to l-1 do
     //   if (i + 1 < l) then
            let x = u.[i]  
            let li = new List<string * float>()  
            [i+1..l-1]
                |> Seq.map (fun j -> (u.[j], fst (process_elem x u.[j] where)) )     
                |> Seq.sortBy (fun (x, y) -> y)
                |> Seq.take 10
                |> Seq.iter (fun (x, y) -> li.Add(x, y))

            printfn "%i" i
            dic.Add (x, li)
    dic

let process_temp x y where = 
    0.1

let matrix_async (u:string[]) (process_elem) where= 
    let l = 100 // u.Length - 1   
    
    seq {             
        for i in [0..l] do            
            if (i + 1 < l) then
                let x = u.[i]                
                for j in [i+1..l] do   
                    yield async { 
                        if (j = i) then printfn "%i" i
                        let y = u.[j]    
                        return (x, y, fst (process_elem x y where))
                    }
        
    }


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

let fill = 
    let se = new SimularityExtract()
    se.Name <- "test"
    se.MatrixU <- matrix (uniq a) sim_jaccard a 
    Kevo.Store.update<SimularityExtract> (0, se)
        

//let first_dic = a |> Seq.head |> snd
//
//let test j = 
//    for i = 1 to 100000 do
//         let a = getf first_dic ("привет" + string i) 
//         if (i % 1000 = 0) then 
//            printfn "%A" a



//let b = Simularity.find_sim a 
    
//b |> printfn "find_sim result = %A"

