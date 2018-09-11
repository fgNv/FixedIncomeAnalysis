module SelicRate

open FSharp.Data

[<Literal>]
let Url = "http://www2.bmf.com.br/pages/portal/bmfbovespa/boletim1/TxRef1.asp?slcTaxa=SLP"

type BovespaSelicData = HtmlProvider<Url>

type Item = {
    Days: string
    Rate: string
}

let private buildRowData (input: HtmlNode * HtmlNode) =
    {
        Days = (fst input).InnerText()
        Rate = (snd input).InnerText()
    }

let tryGet () =
    try
        let html = BovespaSelicData.Load(Url).Html
        let mainContent = html.CssSelect ".tabConteudo" |> Seq.head
        let cells = seq {  yield! mainContent.CssSelect "td.tabelaConteudo1"
                           yield! mainContent.CssSelect "td.tabelaConteudo2"
                        } |> Seq.mapi (fun i x -> (i, x))
        
        let first = cells |> Seq.filter (fun x -> fst x % 2 = 0) |> Seq.map snd
        let second = cells |> Seq.filter (fun x -> fst x % 2 <> 0) |> Seq.map snd
        let duplets = Seq.zip first second
        Models.Success (duplets |> Seq.map buildRowData |> Seq.toList)

    with 
        | ex -> Models.Failure ([ex.Message])
    
    