module Countdown.Tests.LettersTests

open FsCheck
open Countdown.Letters

let solveLettersReturnsValidWords (wordList : WordList) (selection: LetterSelection) =
    let words = solveLetters wordList selection
    List.forall wordList.Contains words

let runLettersTests() =
    do Check.Quick solveLettersReturnsValidWords
