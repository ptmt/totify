module Totify.Protocol

type Token = {
    Id: int;
    Content: string;
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