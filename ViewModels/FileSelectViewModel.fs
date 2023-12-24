namespace ScriptOverview.ViewModels

open ReactiveUI
open System.Windows.Input
open Avalonia.Interactivity
open Avalonia.Controls

type FileSelectViewModel() as this =
    inherit ViewModelBase()

    member val Greeting = "Welcome to Script Overview! Please select a file to get details on it."


