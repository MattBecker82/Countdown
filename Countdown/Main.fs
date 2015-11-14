module Countdown.Main

open System
open Gtk
open MainWindow
open Letters
            
[<EntryPoint>]
let Main(args) = 
    Application.Init()
    let wordList = loadWordList "dictionary.txt" // TODO : Load from resource
    let win = new MainWindow(wordList)
    win.Show()
    Application.Run()
    0
