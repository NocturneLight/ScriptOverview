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
        let file = GetReadFromFile sender
        let extension = Regex.Match(file.Value.Name, FileExtensionRegex)

        // Parses the corresponding document if it is supported.
        match file with
        | Some(_) ->  
            match extension.Value with
            // Checks if the document is the supported Word document.
            | ".docx" | ".doc" ->
                ()
            
            // TODO: Check if it's a text document.
            | ".txt" ->
                raise (NotImplementedException "Support for .txt files is not yet implemented.")
            
            // TODO: Check if it's a pdf document.
            | ".pdf" ->
                raise (NotImplementedException "Support for .pdf files is not yet implemented.")
            
            // Throw an error in any other case.
            | _ -> 
                raise (NotSupportedException "The file type you chose is not supported.")
        
        | None -> 
            ()
        
        // Switches the view from this one to File Overview.
        (sender.DataContext :?> WindowViewModelBase).GoToView(FILEOVERVIEW, file.Value)

    // On creation of the object, creates a reactive command for selecting a file. 
    do _SelectFileCommand <- ReactiveCommand.Create<Window>(SelectFile)
        
    member val Greeting = "Welcome to the Script Overview program! Please select a file to work with."

    // Getters and setters go here.
    member this.SelectFileCommand 
        with get() = _SelectFileCommand
    