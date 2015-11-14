module Countdown.Letters

open System

type LetterSelection = string
type Word = string

// Trim, convert to upper case, sort and convert to list
let normalize (str: string) =
    (str.Trim().ToUpperInvariant()).ToCharArray()
        |> Array.sort
        |> List.ofArray

// Return true iff left is a sublist of right
let rec isSubListOf (left: 'T list) (right: 'T list) =
    match left,right with
    | [],_ -> true
    | _,[] -> false
    | (x::xs),(y::ys) ->
        if x = y
            then isSubListOf xs ys
            else isSubListOf (x::xs) ys

// Return true iff word is an anagram of some subset of letters from selection
let isFromSelection (selection: LetterSelection) (word: Word) =
    isSubListOf (normalize word) (normalize selection)

type WordList = Set<Word>

// Load word list from file (already normalized) and place in set
let loadWordList (filePath : string) =
    let lines = System.IO.File.ReadLines(filePath)
    new WordList(lines)

// Represent the score from a letters game
type LettersGameResult =
    | Valid of int
    | WordNotInList
    | WordNotFromSelection

let score (result: LettersGameResult) =
    match result with
    | Valid x -> x
    | _ -> 0

let scoreMessage (result: LettersGameResult) =
    match result with
    | Valid x -> sprintf "%d" x
    | WordNotInList -> "0 (word not in word list)"
    | WordNotFromSelection -> "0 (word could not be formed from selected letters)"

// Compute the score for a given word and selection
let lettersGameResult (wordList : WordList) (selection: LetterSelection) (word: Word) =
    if not (isFromSelection selection word) then
        WordNotFromSelection
    elif not (wordList.Contains(word)) then
        WordNotInList
    else
        let l = word.Trim().Length
        if l < 9 then (Valid l) else (Valid (2*l))
