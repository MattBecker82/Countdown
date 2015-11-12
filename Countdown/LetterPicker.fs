module Countdown.LetterPicker

type LetterPicker =
    {
        pickConsonant : unit -> char
        pickVowel : unit -> char
    }

let letterPicker consonantPicker vowelPicker =
    {
        pickConsonant = consonantPicker
        pickVowel = vowelPicker
    }

let constPicker c = c

type Random = unit -> float

let uniform seed : Random = (new System.Random(seed)).NextDouble

let distributionPicker (rand : Random) (ds : (char * float) list) =
    let totalWt = ds |> List.sumBy (fun (c,wt) -> wt)
    fun () ->
        let rec pick ds' x =
            match ds' with
                | [] -> failwith "Empty distribution"
                | [(c,wt)] -> c
                | ((c,wt)::tail) -> if x <= wt then c else pick tail (x-wt)
        rand() * totalWt |> pick ds

let uniformPicker (rand : Random) (str: string) =
    str |> Seq.toList |> List.map (fun c -> c,1.0) |> distributionPicker rand
