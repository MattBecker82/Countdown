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

// From Wikipedia
let englishVowelFrequencies =
    [
        'E',12.702;
        'A',8.167;
        'O',7.507;
        'I',6.966;
        'U',2.758
    ]

let englishConsonantFrequencies =
    [
        'T',9.056;
        'N',6.749;
        'S',6.327;
        'H',6.094;
        'R',5.987;
        'D',4.253;
        'L',4.025;
        'C',2.782;
        'M',2.406;
        'W',2.361;
        'F',2.228;
        'G',2.015;
        'Y',1.974;
        'P',1.929;
        'B',1.492;
        'V',0.978;
        'K',0.772;
        'J',0.153;
        'X',0.150;
        'Q',0.095;
        'Z',0.074
    ]

let englishVowelPicker (rand: Random) =
    distributionPicker rand englishVowelFrequencies

let englishConsonantPicker (rand: Random) =
    distributionPicker rand englishConsonantFrequencies

let englishPicker (rand: Random) =
    letterPicker (englishConsonantPicker rand) (englishVowelPicker rand)
