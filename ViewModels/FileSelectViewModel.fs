namespace ScriptOverview.ViewModels

open ReactiveUI
open System.Windows.Input
open Avalonia.Controls
open ScriptOverview.Models.Utilities
open System
open System.Text.RegularExpressions
open ScriptOverview.Models.RegexFilters
open NLog

type FileSelectViewModel() =
    inherit ViewModelBase()

    let _Log = LogManager.GetCurrentClassLogger()

    let mutable _SelectFileCommand: ICommand = null

    // Lets the user select a file asynchronously.
    let SelectFile(sender: Window) =
        try
            // Gets information on the file or nothing.
            let file = GetReadFromFile sender
        
            // Returns the file extension or nothing.
            let extension = 
                file 
                |> Option.map(fun x -> 
                    Regex.Match(x.Name, FileExtensionRegex))
        
            // Parses the corresponding document if it is supported.
            match extension with
            // Switches the view from this one to File Overview if the file type is supported.
            | Some ext when ext.Value = ".docx" || ext.Value = ".doc" ->
                (sender.DataContext :?> WindowViewModelBase).GoToView(FILEOVERVIEW, file.Value)

            // Logs to the event log that the file type is not yet implemented.
            | Some ext when ext.Value = ".txt" || ext.Value = ".pdf" ->
                _Log.Info(NotImplementedException(), $"Support for {ext.Value} files has not yet been implemented.")

            // Logs to the event log that the file they chose is not supported.
            | Some ext when ext.Value <> ".docx" 
                || ext.Value <> ".doc" 
                || ext.Value <> ".txt" 
                || ext.Value <> ".pdf" ->
            
                let message = "The file you have chosen is not supported."

                _Log.Info(NotSupportedException(), message)
            | _ ->
                ()
        with
        | ex ->
            _Log.Error(ex, "An error occurred when the user tried to select a file.")

    // On creation of the object, creates a reactive command for selecting a file. 
    do 
        try
            _SelectFileCommand <- ReactiveCommand.Create<Window>(SelectFile)
        with
        | ex ->
            _Log.Error(ex, "An error occurred trying to create Reactive Commands for the FileSelectViewModel.")
        
    member val Greeting = "Welcome to the Script Overview program! Please select a file to work with."

    // Getters and setters go here.
    member this.SelectFileCommand 
        with get() = _SelectFileCommand
    