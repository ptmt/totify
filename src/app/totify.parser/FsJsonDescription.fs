module FsJsonDescription
open FsJson
type JTree = JObj of (string*JTree)array | JArr of JTree array | Js | Jf | Ji | Jb | Jempty
let describeTo max (j:Json) = 
    let rec loop depth max (j:Json)  =
        match j with
            | JsonObject pMap ->
                if depth < max then
                    pMap |> Map.toArray |> Array.map (fun (k,v) -> (k, v |> loop (depth + 1) max)) |> JObj
                else
                    JObj[||]
            | JsonArray arr ->
                if depth < max then
                    arr |> Array.map (fun av -> av |> loop (depth + 1) max) |> JArr
                else
                    JArr[||]
            | JsonBool _ -> Jb
            | JsonFloat _ -> Jf
            | JsonInt _ -> Ji
            | JsonString _ -> Js
            | JsonNull _ -> Jempty
    loop 0 max j
let describe j = describeTo System.Int32.MaxValue j
type Json with
    member x.Describe = x |> describe
    member x.DescribeTo(dept) = x |> describeTo  dept

//not tested for valid serialization
let format (json:Json) =
    let sb = System.Text.StringBuilder()
    let (!>) (s:string) = sb.Append(s) |> ignore
    let tabs n = for i in 1..n do sb.Append(" ") |> ignore
    let line n = sb.AppendLine() |> ignore; tabs n; 
    let appS (s:string) = 
        !>"\""
        for i in 0..s.Length-1 do 
            let c = s.[i]
            match c with
            | '"'  -> !> "\\\""
            | '\b' -> !>"\\b"
            | '\f' -> !>"\\f"
            | '\n' -> !>"\\n"
            | '\r' -> !>"\\r"
            | '\t' -> !>"\\t"
            | x    -> sb.Append(x) |> ignore
        !>"\""
    let rec loop j d =
        match j with
        | JsonObject pMap -> 
            if Map.isEmpty pMap then !>"{}"
            else
                line d
                !>"{"
                line d
                pMap |> Map.iter (fun k v -> appS k; !>":";  loop v (d+1); !>", "; line d)
                sb.Length <- sb.Length - (4+d)
                line d
                !>"}"            
        | JsonString str -> appS str
        | JsonFloat n    -> !> n.ToString()
        | JsonInt n      -> !> n.ToString()
        | JsonBool b     -> if b then !>"true" else !>"false"
        | JsonNull       -> !>"null"
        | JsonArray arr  ->
            if Array.isEmpty arr then !>"[]"
            else
                line d
                !>"["
                line d
                arr |> Array.iter (fun j -> loop j (d+1); !>", "; line d)
                sb.Length <- sb.Length - (4+d)
                line d
                !>"]"
    loop json 0
    sb.ToString()
