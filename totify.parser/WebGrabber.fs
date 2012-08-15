module Totify.Parser.WebGrabber

open System.Json
// https://github.com/d1ffuz0r/2ch-API/blob/master/api2ch.py

let dvach_url = "http://2ch.so/"
let boards_api = "/wakaba.json"
let dvach_boards = ["s"; "pr"; "fiz"; "b"; "sex"; "app"; "g"; "dev"; "ew"; "biz"; "bo"; "di";
 "gd"; "mu"; "psy"; "sn";"spc"; "t"; "bg";"p";"pa";"di"; "gd"; "fl"; "me" ;"mg";
 "mo"; "mlp"; "soc"; "fag";"a"; "ja"; "w"]
//let dvach_boards = ["s"; "pr"; "fiz" ]


type a = System.Json.JsonValue list

let get_threads_list board = 
    async {        
        use wc = new System.Net.WebClient()
        wc.Encoding <- System.Text.Encoding.UTF8        
        let durl = dvach_url + board + boards_api
        printfn "downloading threads list from %A" durl
        return (board, wc.DownloadString (System.Uri( durl )))
        
    }

/// проверяем есть ли скаченный тред, если да, то возвращаем пустую строку
let get_thread board num = 
    async {
        use wc = new System.Net.WebClient()
        wc.Encoding <- System.Text.Encoding.UTF8        
        let durl = dvach_url + board + "/res/" + num + ".json"
        printfn "downloading posts from %A" durl
        return  wc.DownloadString (System.Uri( durl ))
    }

let dvach_grabber (indexed_threads:string list) = 
    let dv_threads_json = dvach_boards |> Seq.ofList |> Seq.map (fun x -> get_threads_list x) |> Async.Parallel |> Async.RunSynchronously
   // use f = System.IO.File.CreateText("c:\\data\\dvach.json")   
    let js = snd dv_threads_json.[0] |> FsJson.parse

    let process_thread (board) (thread:FsJson.Json) = 
        let num = thread?posts.Array.[0].Array.[0]?num.Val
        let l = List.tryFind  (fun x-> x = num) indexed_threads  
        if l.IsSome then
            (string num, [|""|])
        else 
            let thread_json = get_thread board num |> Async.RunSynchronously |> FsJson.parse
            (string num, thread_json?thread.Array.[0].Array |> Array.map (fun (x:FsJson.Json) -> string x?comment.Val))
            //|> Array.fold (fun (acc:string) (comment:string) -> acc + comment) "") )
        
    let each_board board board_json_raw = 
      //  (board, "")
        //use f = System.IO.File.CreateText("c:\\data\\" + board + ".json")   
        //f.Write(string board_json_raw)
        let board_json = FsJson.parse board_json_raw
        (board, board_json?threads.Array |> Array.fold (fun acc th -> 
                                let r = process_thread board th 
                                let threads_text = snd r |> Array.fold (fun (acc:string) (comment:string) -> acc + comment) "" 
                                (fst acc @ [fst r], snd acc + threads_text)) 
             (indexed_threads, string "")
        )

    let posts = 
        dv_threads_json 
        |> Seq.map (fun (board, board_json_raw) -> each_board board board_json_raw)
       // |> Seq.collect (fun board -> board?threads.Array |> Array.map process_thread )
       // |> Seq.collect (fun x -> x)
        
   
    //f.Write((sprintf "%A" (js |> FsJsonDescription.describe) ))

    posts