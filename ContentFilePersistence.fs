module ContentFilePersistence

open System.IO
open Newtonsoft.Json
open System

let persist content objectName =
    let fileName = sprintf "%s_%s.json" (DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss")) objectName    
    let stringContent = JsonConvert.SerializeObject(content)
    File.WriteAllText(fileName, stringContent)