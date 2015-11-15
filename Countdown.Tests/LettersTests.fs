module Countdown.Tests.LettersTests

open FsCheck
open Countdown.Letters

let letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray() |> Seq.ofArray
let letterGen = Gen.elements letters

let wordGen =
    Gen.nonEmptyListOf letterGen
        |> Gen.map (fun lChars -> System.String.Concat(Array.ofList(lChars)))

let wordListGen : Gen<WordList> =
    Gen.nonEmptyListOf wordGen
        |> Gen.map (fun lWords -> new WordList(lWords))

let selectionGen : Gen<LetterSelection> =
    Gen.arrayOfLength 9 letterGen
        |> Gen.map (fun aChars -> System.String.Concat(letterGen))

type LettersGenerators =
  static member LetterSelection() =
      {new Arbitrary<LetterSelection>() with
          override x.Generator = selectionGen
          override x.Shrinker ls = Seq.empty }
   
  static member WordList() =
      {new Arbitrary<WordList>() with
          override x.Generator = wordListGen
          override x.Shrinker wl = Seq.empty }

do Arb.register<LettersGenerators>() |> ignore

let solveLettersReturnsValidWords (wordList : WordList) (selection: LetterSelection) =
    let words = solveLetters wordList selection
    let isValid word =
        (isFromSelection selection word) && (wordList.Contains(word))
    List.forall isValid words

let solveLettersReturnsAllSameLength (wordList : WordList) (selection: LetterSelection) =
    let words = solveLetters wordList selection
    match words with
        | [] -> true
        | (w::ws) ->
            let len = String.length w
            ws |> List.map String.length |> List.forall (fun l -> l = len)

let solveLettersAreLongerThanOrEqualToMatches (wordList : WordList) (selection: LetterSelection) =
    let words = solveLetters wordList selection
    let allMatches = wordList |> Seq.filter (isFromSelection selection)
    if Seq.isEmpty allMatches then
        words.IsEmpty
    else
        match words with
        | [] -> false
        | _ ->
            let minLen = words |> List.map String.length |> List.min
            Seq.forall (fun (w : Word) -> w.Length <= minLen) allMatches    

let solveLettersIfMatchMissingItMustBeShorter (wordList : WordList) (selection: LetterSelection) =
    let words = new Set<Word>(solveLetters wordList selection)
    let allMatches = wordList |> Seq.filter (isFromSelection selection)
    if Seq.isEmpty allMatches then
        words.IsEmpty
    else
        if Seq.isEmpty words then
            false
        else
            let maxLen = words |> Seq.map String.length |> Seq.max
            Seq.forall (fun (w : Word) -> words.Contains(w) || w.Length < maxLen) allMatches    

let runLettersTests() =
    do Check.Quick solveLettersReturnsValidWords
    do Check.Quick solveLettersReturnsAllSameLength
    do Check.Quick solveLettersAreLongerThanOrEqualToMatches
    do Check.Quick solveLettersIfMatchMissingItMustBeShorter
