module Totify.Tests.SynonymizerTests

open NUnit.Framework  
open TinyNLP
open Totify.Tests.Helpers

 
[<Test>]
let ``synonymizer should not be able to synonymize usual unstemmed word`` () =
    TinyNLP.Synonymizer.getSynonyms "слово" |> fun x -> x.Length = 0 |> shouldBeTrue

[<Test>]
let ``synonymizer should not be able to synonymize unknown word`` () =
    TinyNLP.Synonymizer.getSynonyms "adfadfa23" |> fun x -> x.Length = 0 |> shouldBeTrue

[<Test>]
let ``synonymizer should not be able to synonymize stemmed word`` () =
    TinyNLP.Synonymizer.getSynonyms (TinyNLP.Stemming.Stem "слово") |> fun x -> x.Length > 1 |> shouldBeTrue