#if INTERACTIVE
#time
#I @"D:\projects\totify\packages\FAKE.1.64.18.0\tools\"
#r "FakeLib.dll"
#endif
module Totify.WebDeploy

// include Fake libs

open Fake
open System

let test_build =    
    let startInfo = new System.Diagnostics.ProcessStartInfo();
    startInfo.FileName <- @"D:\projects\totify\tools\FAKE\Fake.exe"
    startInfo.Arguments <- @"D:\projects\totify\build.fsx"
    startInfo.RedirectStandardOutput <- true
    startInfo.UseShellExecute <- false
    startInfo.WorkingDirectory <- @"D:\projects\totify\"
    use buildProcess = System.Diagnostics.Process.Start startInfo
    use reader = buildProcess.StandardOutput
    let result = reader.ReadToEnd()
    result