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

// Compute the score for a given word and selection
// TODO: Take into account dictionary
let score (selection: LetterSelection) (word: Word) =
    if isFromSelection selection word
        then word.Trim().Length
        else 0
