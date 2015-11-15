module Countdown.LettersGameWindow

open System
open System.Threading.Tasks
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

    let answersButton = new Button("Answers")
    do answersButton.Clicked.AddHandler(fun o e -> this.AnswersClicked(o, e))
    do answersButton.Sensitive <- false
    do vbox.PackStart(answersButton)

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

    let mutable (solverTask : Task<Word list>) = null

    let stopClock() =
        do userWord.Sensitive <- false
        do progressBar.Sensitive <- false
        do userWordListView.Sensitive <- true
        do answersButton.Sensitive <- true
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

    let showMessage (msg : string) =
        let msgBox = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, msg)
        do msgBox.Run() |> ignore
        do msgBox.Destroy()
        ()

    let showScore (word : string) =
        let word' = word.Trim().ToUpper()
        let sc = lettersGameResult wordList sourceLetters.Text word' |> scoreMessage
        let msg = sprintf "Letters: %s\nYour word: %s\nScore: %s" sourceLetters.Text word' sc
        showMessage msg

    let showAnswers() =
        do solverTask.Wait()
        let answers =
            solverTask.Result
                |> List.map (fun s -> sprintf " %s (%d)" s s.Length)

        let answersMsg =
            match answers with
                | [] -> "No answers"
                | [x] -> sprintf "Best answer:%s" x
                | _ -> sprintf "Best answers:\n%s" (String.concat "\n" answers)
        let msg = sprintf "Letters: %s\n%s" sourceLetters.Text answersMsg
        showMessage msg

    member this.ConsonantClicked(o, e) =
        letterPicker.pickConsonant() |> addCharToSource

    member this.VowelClicked(o, e) =
        letterPicker.pickVowel() |> addCharToSource

    member this.StartClicked(o, e) =
        do userWord.Sensitive <- true
        do userWord.GrabFocus()
        do progressBar.Sensitive <- true
        do startButton.Sensitive <- false
        let solver = async { return solveLetters wordList sourceLetters.Text }
        solverTask <- Async.StartAsTask(solver)
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

    member this.AnswersClicked(o, e) =
        do userWordListView.Sensitive <- false
        showAnswers()