module Totify.Tests.Filter2Tests

open NUnit.Framework  
open TinyNLP
open Totify.Tests.Helpers

[<Test>]
let ``filter 2 should able to add new rules`` () =   
    let i = Kevo.Store.lastid
    Totify.Filters.Filter2.insertRule "какбэ" ""
    Totify.Filters.Filter2.insertRule "ну" ""
    System.Threading.Thread.Sleep(1000)
    Kevo.Store.lastid - i = 2 |> shouldBeTrue