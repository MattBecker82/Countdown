module Countdown.LettersGameWindow

open System
open GLib
open Gtk
open Countdown.LetterPicker
open Countdown.Ticker

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

    let progressBar = new ProgressBar()
    do progressBar.Fraction <- 0.0
    do progressBar.Sensitive <- false
    do vbox.PackStart(progressBar)

    let userWord = new Entry()
    do userWord.Sensitive <- false
    do vbox.PackStart(userWord)

    let userWordList = new Gtk.TreeView(new ListStore(GLib.GType.String))
    do new TreeViewColumn("Your words", new Gtk.CellRendererText())
        |> userWordList.AppendColumn
        |> ignore
    do userWordList.Sensitive <- false
    do vbox.PackStart(userWordList)

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
        do userWordList.Sensitive <- false
        do progressBar.Sensitive <- false
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

    member this.ConsonantClicked(o, e) =
        letterPicker.pickConsonant() |> addCharToSource

    member this.VowelClicked(o, e) =
        letterPicker.pickVowel() |> addCharToSource

    member this.StartClicked(o, e) =
        do userWord.Sensitive <- true
        do userWordList.Sensitive <- true
        do progressBar.Sensitive <- true
        do startButton.Sensitive <- false
        startClock()

