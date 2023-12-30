namespace ScriptOverview.ViewModels

open ReactiveUI
open System.Windows.Input
open Avalonia.Controls
open System
open Avalonia.Platform.Storage
open DocumentFormat.OpenXml.Packaging
open DocumentFormat.OpenXml.Wordprocessing
open System.IO
open System.Diagnostics

type FileSelectViewModel() as this =
    inherit PageViewModelBase()

    let mutable _CanNavigateNext: bool = true
    let mutable _CanNavigatePrevious: bool = false
    let mutable _SelectFileCommand: ICommand = null
    let mutable _FileName = "The name of the file you chose will show here once selected!"
    let mutable _DocumentBody: Body = null

    // Lets the user select a file ansynchronously.
    let SelectFile(sender: Window) =
        // Task that when ran, lets the user pick a file on their computer.
        let task = async {
            let topLevel = TopLevel.GetTopLevel sender
            
            let options = 
                FilePickerOpenOptions() 
                |> fun option -> 
                    option.Title <- "Select File"
                    option.AllowMultiple <- false
                    
                    option
            
            return! topLevel.StorageProvider.OpenFilePickerAsync options |> Async.AwaitTask
        }

        // Runs the task and returns the user's chosen file if it exists, else null.
        let file =
            task
            |> Async.RunSynchronously 
            |> Seq.tryExactlyOne

        match file with
        | Some(doc) -> 
            this.FileName <- doc.Name
            
            try
                _DocumentBody <- WordprocessingDocument.Open(doc.Path.LocalPath, false).MainDocumentPart.Document.Body
            with
                | :? FileFormatException -> 
                    // Outputs an error, then closes the program.
                    let message = "The file you tried to open was not a .docx file."
                    
                    Debug.WriteLine message
                    sender.Close()

            (sender.DataContext :?> ViewModelBase).IsNavigationVisible <- true
            
        | None -> 
            ()

    // On creation of the object, creates a reactive command for selecting a file. 
    do _SelectFileCommand <- ReactiveCommand.Create<Window>(SelectFile)
        

    member val Greeting = "Welcome to Script Overview! Please select a file to get details on it."

    // Getters and setters go here.
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
        
        and set(path) = 
            this.RaiseAndSetIfChanged(&_FileName, path) |> ignore
    
    member this.SelectFileCommand 
        with get() = _SelectFileCommand

    member this.DocumentBody
        with get() = _DocumentBody
    