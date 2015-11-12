namespace Countdown

module LettersGameWindow =
    open System
    open Gtk
    open Countdown.LetterPicker
    
    type LettersGameWindow(letterPicker: LetterPicker) as this =
        inherit Window("Letters Game")

        do this.SetDefaultSize(400, 300)

        let vbox = new VBox(true, 5)

        let sourceLetters = new Label("")
        do vbox.PackStart(sourceLetters)

        let hbox = new HBox(true, 5)

        let consonantButton = new Button("Consonant")
        do consonantButton.Clicked.AddHandler(fun o e -> this.ConsonantClicked(o, e))
        do hbox.PackStart(consonantButton)

        let vowelButton = new Button("Vowel")
        do vowelButton.Clicked.AddHandler(fun o e -> this.VowelClicked(o, e))
        do hbox.PackStart(vowelButton)

        let startButton = new Button("Start Timer")
        do startButton.Clicked.AddHandler(fun o e -> this.StartClicked(o, e))
        do startButton.Sensitive <- false
        do hbox.PackStart(startButton)

        do vbox.PackStart(hbox)

        do this.Add(vbox)

        do this.ShowAll()

        let checkSourceLength() =
            if (String.length sourceLetters.Text) >= 9 then
                do consonantButton.Sensitive <- false
                do vowelButton.Sensitive <- false
                do startButton.Sensitive <- true
            
        let addCharToSource c =
            do sourceLetters.Text <- sprintf "%s%c" sourceLetters.Text c
            do checkSourceLength()
            
        member this.ConsonantClicked(o, e) =
            letterPicker.pickConsonant() |> addCharToSource

        member this.VowelClicked(o, e) =
            letterPicker.pickVowel() |> addCharToSource

        member this.StartClicked(o, e) =
            ()
