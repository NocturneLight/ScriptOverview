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

type FileSelectViewModel() as this =
    inherit PageViewModelBase()

    let mutable _CanNavigateNext: bool = true
    let mutable _CanNavigatePrevious: bool = false
    let mutable _SelectFileCommand: ICommand = null
    let mutable _FileName = "The name of the file you chose will show here once selected!"
    let mutable _DocumentBody: Body = null

    // Lets the user select a file ansynchronously.
    let SelectFile(sender: Window) =
        let file = ReadFromFileInfo sender

        // Parses the corresponding document if it is supported.
        match file with
        | Some(doc) ->  
            let extension = doc.Name.Split '.'
            
            this.FileName <- doc.Name
            
            match Array.last extension with
            // Parses a Word document.
            | "docx" | "doc" ->
                try
                    use document = WordprocessingDocument.Open(doc.Path.LocalPath, false)

                    _DocumentBody <- 
                        document.MainDocumentPart.Document.Body
                with
                    // Outputs the corresponding error and then closes the program.
                    | :? ArgumentNullException -> 
                        Debug.WriteLine $"The file path was somehow set to null. The path used has the value: {doc.Path.LocalPath}"
                        sender.Close()
                    
                    | :? OpenXmlPackageException ->
                        Debug.WriteLine $"The file you tried to open seems to not be a valid Open XML Wordprocessing document."
                        sender.Close()
            
            // TODO: Parse a text document.
            | "txt" ->
                raise (NotImplementedException "Support for .txt files is not yet implemented.")
            
            // TODO: Parse a pdf document.
            | "pdf" ->
                raise (NotImplementedException "Support for .pdf files is not yet implemented.")
            
            // Throw an error in any other case.
            | _ -> 
                raise (NotSupportedException "The file type you chose is not supported.")
           
            // Enables the navigation buttons.
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
    