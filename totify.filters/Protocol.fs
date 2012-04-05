module Totify.Protocol



type Token = {
    Id: int;
    Content: string;
    Class:TinyNLP.Tokenizer.TokenClass;
}

type ChangeType = 
    | Replace
    | Other


type Change = {
    Variants : string list;    
}

type Node = {
    Token: Token;
    Changes: Change list;
  //  Childs: Node list;
}