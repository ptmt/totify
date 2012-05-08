module Totify.NLP

let initTagger =  
    
    let corpus_data = 
        Kevo.Store.memo<TinyNLP.POST.Corpus.CorpusData> (fun () -> 
            use f = System.IO.File.OpenText("C:\\data\\annot.opcorpora.xml")
            TinyNLP.POST.Corpus.readCorpus f
            )    
    let sf = 
        Kevo.Store.memo<TinyNLP.POST.Suffix.SuffixTree> (fun () -> TinyNLP.POST.Suffix.buildSuffixTree corpus_data)
    let mp = 
        Kevo.Store.memo<TinyNLP.POST.Word.mapprobs> (fun () -> TinyNLP.POST.Word.getWordProbs corpus_data)
   
    (corpus_data, sf, mp)

let tagTokens tokens = 
    TinyNLP.POST.Tagger.viterbi tokens initTagger
        |> TinyNLP.POST.Tagger.highestProbabilitySequence
        |> List.tail        
        |> List.map (fun x -> x.Split(',') |> List.ofArray)
        

