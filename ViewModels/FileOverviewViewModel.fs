namespace ScriptOverview.ViewModels

open ReactiveUI
open DocumentFormat.OpenXml.Wordprocessing

type FileOverviewViewModel() =
    inherit PageViewModelBase()
    
    let mutable _CanNavigateNext = false
    let mutable _CanNavigatePrevious = true
    let mutable _FileName = ""

    member val FileContents: Body = null with get, set

    override this.CanNavigateNext 
        with get() = _CanNavigateNext
        
        and set(value) = 
            this.RaiseAndSetIfChanged(&_CanNavigateNext, value) |> ignore

    override this.CanNavigatePrevious
        with get() = _CanNavigatePrevious

        and set(value) =
            this.RaiseAndSetIfChanged(&_CanNavigatePrevious, value) |> ignore
    
    member this.FileName
        with get() = _FileName

        and set(message) =
            this.RaiseAndSetIfChanged(&_FileName, message) |> ignore


            