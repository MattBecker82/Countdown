﻿module Countdown.MainWindow

open System
open Gtk
open LetterPicker
open Letters
open LettersGameWindow

type MainWindow(wordList : WordList) as this = 
    inherit Window("Countdown")
    do this.SetDefaultSize(400, 300)
    do this.DeleteEvent.AddHandler(fun o e -> this.OnDeleteEvent(o, e))

    let lettersButton = new Button("Letters")
    do lettersButton.Clicked.AddHandler(fun o e -> this.OnLettersClicked(o, e))
    do this.Add(lettersButton)

    do this.ShowAll()

    member this.OnDeleteEvent(o, e : DeleteEventArgs) = 
        Application.Quit()
        e.RetVal <- true

    member this.OnLettersClicked(o, e) =
        let rand = uniform (int (DateTime.Now.ToFileTime()))
        let lp = englishPicker rand
        let win = new LettersGameWindow(wordList, lp)
        do win.Show()
