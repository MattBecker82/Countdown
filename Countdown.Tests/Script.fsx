#I "../packages/FsCheck.2.2.2/lib/net45"
#I "../Countdown/bin/Debug"
#I "/usr/lib/cli/gtk-sharp-2.0"
#r "FsCheck"
#r "Countdown"
#load "LettersTests.fs"

open Countdown.Tests.LettersTests

let runAllTests() =
    do runLettersTests()
    ()

runAllTests()
