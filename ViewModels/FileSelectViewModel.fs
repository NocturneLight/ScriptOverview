namespace ScriptOverview.ViewModels

open ReactiveUI
open System.Windows.Input
open Avalonia.Controls
open Avalonia.Platform.Storage
open DocumentFormat.OpenXml.Wordprocessing
open ScriptOverview.Models.Utilities
open System
open System.Diagnostics
open DocumentFormat.OpenXml.Packaging
open System.Text.RegularExpressions
open ScriptOverview.Models.RegexFilters

type FileSelectViewModel() =
    inherit ViewModelBase()

    let mutable _SelectFileCommand: ICommand = null

    // Lets the user select a file asynchronously.
    let SelectFile(sender: Window) =
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

        // Raise an exception if the file type is not supported.
        | Some ext when ext.Value = ".txt" || ext.Value = ".pdf" ->
            raise (NotImplementedException $"Support for {ext.Value} files has not yet been implemented.")

        // Informs the user that the file they chose is not supported.
        | Some ext when ext.Value <> ".docx" 
            || ext.Value <> ".doc" 
            || ext.Value <> ".txt" 
            || ext.Value <> ".pdf" ->
            
            raise (NotSupportedException "The file you have chosen is not supported.")

        | _ ->
            ()

    // On creation of the object, creates a reactive command for selecting a file. 
    do _SelectFileCommand <- ReactiveCommand.Create<Window>(SelectFile)
        
    member val Greeting = "Welcome to the Script Overview program! Please select a file to work with."

    // Getters and setters go here.
    member this.SelectFileCommand 
        with get() = _SelectFileCommand
    