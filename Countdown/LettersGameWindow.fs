namespace Countdown

module LettersGameWindow =
    open System
    open Gtk
    
    type LettersGameWindow() as this = 
        inherit Window("Letters Game")
        do this.SetDefaultSize(400, 300)
        do this.ShowAll()
