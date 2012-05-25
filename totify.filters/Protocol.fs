module Totify.Protocol



type Token = {
    Id: int;
    Content: string;
    Class:TinyNLP.Tokenizer.TokenClass;
    Tag:string list;
}

type ChangeType = 
    | Replace
    | Other


type Change = {
    FilterName : string;
    Variants : string list;    
}

type Node = {
    Token: Token;
    Changes: Change list;
  //  Childs: Node list;
}