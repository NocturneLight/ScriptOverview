namespace ScriptOverview.ViewModels

open ReactiveUI
open System
open DocumentFormat.OpenXml.Wordprocessing
open Avalonia.Interactivity

type FileOverviewViewModel() =
    inherit ViewModelBase()
    
    member val Greeting = "Hi!" with get, set
    member val FileContents: Body = null with get, set


        
            