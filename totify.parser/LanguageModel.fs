module Totify.Parser.LanguageModel

open System.Collections.Generic
open ProtoBuf

type Probs = { 
    Unigrams:Dictionary<string, int>;
    Bigrams:Dictionary<string, int>;
    Trigrams:Dictionary<string, int>;
    Size:int
    }




let inline incDict (dict:Dictionary<'a, int>, key:'a) = 
    if dict.ContainsKey(key) then         
        dict.Item key <- dict.Item key + 1
    else
        dict.Add(key, 1)
    dict

let delimiter = " "    

let handleSentence (sentence:string list) (a:Probs) :Probs = 
    
    let rec handleWord (c:Probs) (i:int) =        
                
        let getBigrams = 
            if (i > 0) then
                incDict (c.Bigrams, ( (sentence.[i - 1]) + delimiter + (sentence.[i]) ));
            else 
                c.Bigrams
        let getTrigrams = 
            if (i > 1) then
                incDict (c.Trigrams, ( (sentence.[i - 2]) + delimiter + (sentence.[i - 1]) + delimiter + (sentence.[i]) ));
            else 
                c.Trigrams

        if i = sentence.Length - 1 then
            c
        else            
            let probs = {
                //Lexicon = getLexicon (c.Lexicon, sentence.[i]);
                Unigrams =  incDict (c.Unigrams, (sentence.[i]));
                Bigrams =  getBigrams;
                Trigrams = getTrigrams;
                Size = c.Size + sentence.Length
                }
            handleWord probs (i+1)

    handleWord a 0