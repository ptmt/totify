module Totify.Tests.Filter1Tests

open NUnit.Framework  
open TinyNLP
open Totify.Tests.Helpers

let text1 = "привет всем "
let text2 = "Привет "
let text3 = "Опа, что-то случилось?"
 
[<Test>]
let ``synonymFilter should return changes and token`` () =
    Totify.Filters.Master.prepareText text1 
    |> Totify.Filters.Master.synonymFilter 
    |> (fun x -> x.Head.Changes.Length > 0 && x.Length = 2 && x.Item(0).Token.Content = "привет") 
    |> shouldBeTrue

[<Test>]
let ``synonymFilter should return only word without spaces`` () =
    Totify.Filters.Master.prepareText text2 
    |> Totify.Filters.Master.synonymFilter 
    |> (fun x -> x.Item(0).Changes.Head.Variants.Length > 0 && x.Length = 1 && x.Item(0).Token.Content = "Привет") 
    |> shouldBeTrue

[<Test>]
let ``synonymFilter should correct process commas and dashes`` () =
    Totify.Filters.Master.prepareText text3 
    |> Totify.Filters.Master.synonymFilter 
    |> (fun x -> x.Length = 4 && x.Item(0).Token.Content = "Опа") 
    |> shouldBeTrue
