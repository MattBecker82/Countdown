module Countdown.Main

open System
open Gtk
open MainWindow
            
[<EntryPoint>]
let Main(args) = 
    Application.Init()
    let win = new MainWindow()
    win.Show()
    Application.Run()
    0
