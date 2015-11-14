module Countdown.LettersGameWindow

open System
open GLib
open Gtk
open Countdown.LetterPicker
open Countdown.Letters

type LettersGameWindow(wordList: WordList, letterPicker: LetterPicker) as this =
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

    let progressBar = new ProgressBar()
    do progressBar.Fraction <- 0.0
    do progressBar.Sensitive <- false
    do vbox.PackStart(progressBar)

    let userWord = new Entry()
    do userWord.Activated.AddHandler(fun o e -> this.UserWordActivated(o, e))
    do userWord.Sensitive <- false
    do vbox.PackStart(userWord)

    let userWordListModel = new ListStore(typeof<string>)
    let userWordListView = new TreeView(userWordListModel)
    do userWordListView.RowActivated.AddHandler(fun o e -> this.UserWordListActivated(o, e))
    do new TreeViewColumn("Word", new CellRendererText(), "text", 0)
        |> userWordListView.AppendColumn
        |> ignore
    do userWordListView.Sensitive <- false
    do vbox.PackStart(userWordListView)

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
        
    let mutable elapsed = 0u
    let interval = 100u
    let totalTime = 30000u

    let stopClock() =
        do userWord.Sensitive <- false
        do progressBar.Sensitive <- false
        do userWordListView.Sensitive <- true
        ()
        
    let tick() =
        do elapsed <- min (elapsed + interval) totalTime
        do progressBar.Fraction <- (float elapsed / float totalTime)
        if elapsed >= totalTime then
            do stopClock()
        elapsed < totalTime

    let startClock() =
        do elapsed <- 0u
        GLib.Timeout.Add(interval, new GLib.TimeoutHandler(tick)) |> ignore

    let enterWord (word : string) =
        do userWordListModel.AppendValues(word, "tba") |> ignore
        userWord.Text <- ""

    let showScore (word : string) =
        let word' = word.Trim().ToUpper()
        let sc = lettersGameResult wordList sourceLetters.Text word' |> scoreMessage
        let msg = sprintf "%s\nYour word: %s\nScore: %s" sourceLetters.Text word' sc
        let msgBox = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, msg)
        do msgBox.Run() |> ignore
        do msgBox.Destroy()
        ()

    member this.ConsonantClicked(o, e) =
        letterPicker.pickConsonant() |> addCharToSource

    member this.VowelClicked(o, e) =
        letterPicker.pickVowel() |> addCharToSource

    member this.StartClicked(o, e) =
        do userWord.Sensitive <- true
        do userWord.GrabFocus()
        do progressBar.Sensitive <- true
        do startButton.Sensitive <- false
        startClock()
    
    member this.UserWordActivated(o, e) =
        let word = userWord.Text
        if word.Length > 0 then
            do enterWord word
    
    member this.UserWordListActivated(o, e) =
        let mutable model = null :> TreeModel
        let mutable iter = new TreeIter()
        if userWordListView.Selection.GetSelected(&model, &iter) then
            let word = model.GetValue(iter, 0) :?> string
            showScore word

