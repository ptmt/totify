module Totify.Tests.Filter2Tests

open NUnit.Framework  
open TinyNLP
open Totify.Tests.Helpers
open Totify.Filters.Filter2

[<Test>]
let ``filter 2 should able to add new rules`` () =   
    let i = Kevo.Store.lastid<ReplaceRule>
    Totify.Filters.Filter2.insertRule "какбэ" ""
    Totify.Filters.Filter2.insertRule "ну" ""
    //Kevo.AppendLog.waitForIt<ReplaceRule>
    Kevo.Store.lastid<ReplaceRule> - i = 2 |> shouldBeTrue