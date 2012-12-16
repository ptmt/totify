#if INTERACTIVE
#time
#I @"D:\projects\totify\packages\FAKE.1.64.18.0\tools\"
#r "FakeLib.dll"
#endif
module Totify.WebDeploy.MSBuild

// include Fake libs

open Fake
open System

let test_build = 
    // Properties
    let baseDir = @"D:\projects\totify"
    let buildDir = baseDir + @"\build\"
    let appReferences = !+ (baseDir + @"\**\*.fsproj") 
 

    // Targets
    Target "Clean" (fun _ ->
        CleanDir buildDir
    )

    Target "Initlog" (fun _ -> 
        printfn "Current directory: %A" (System.IO.Directory.GetCurrentDirectory())
    )

    Target "Scan" (fun _ -> 
        appReferences |> Scan |> Seq.iter (printfn "scanned %s")
    )

    Target "BuildApp" (fun _ ->
       appReferences
          |> Scan 
          |> MSBuildRelease buildDir "Build" 
          |> Log "AppBuild-Output: "
    )
    // Dependencies
    "Clean"
      ==> "Initlog"
      ==> "Scan"
      ==> "BuildApp" 

    |> printfn "%A"      

    // start build
    Run "BuildApp"