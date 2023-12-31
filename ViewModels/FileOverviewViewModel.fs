namespace ScriptOverview.ViewModels

open ReactiveUI
open DocumentFormat.OpenXml.Wordprocessing
open System.Windows.Input
open DocumentFormat.OpenXml
open System
open Avalonia.Controls
open ScriptOverview.Models.Utilities

type FileOverviewViewModel() as this =
    inherit PageViewModelBase()
    
    let mutable _CanNavigateNext = false
    let mutable _CanNavigatePrevious = true
    let mutable _FileName = ""
    let mutable _Message = ""
    let mutable _FileBodySummary: string seq = Seq.empty
    let mutable _GetActorsCommand: ICommand = null

    // Function which parses the document for a
    // potential list of actors and writes them to a file.
    let WriteActorsToFile(sender: Window) =
        let body = this.FileContents.ChildElements
        
        // Gets every instance of the Run object.
        let results =
            body
            |> Seq.map(fun e -> 
                Seq.tryFind(fun (x: OpenXmlElement) -> x :? Run) 
                    e.ChildElements)
        
        // Filters the document in various ways looking for potential names.
        let actors =
            results
            // Gets the dialogue string.
            |> Seq.map(fun e -> 
                match e with
                | Some(element) -> 
                    element.InnerText
                | None -> 
                    String.Empty
            )
            // Filters out all strings that don't contains ":".
            |> Seq.where(fun s -> 
                s.Contains ":"
            )
            // Replaces any string that doesn't appear to be a name with the empty string. 
            |> Seq.map(fun s -> 
                let potentialActors = Array.head <| s.Split ":"

                match potentialActors with
                | name when name.Contains "," -> 
                    String.Empty

                | name when name.Contains "." ->
                    String.Empty

                | name when name.ToLower().Contains "sequence" ->
                    String.Empty

                | name when (name.Split " ").Length > 3 && not <| name.ToLower().Contains "same time" ->
                    String.Empty

                | _ ->
                    potentialActors.Trim()
            )
            // Filters out any duplicate strings.
            |> Seq.distinct
            // Filters out any remaining empty strings or null.
            |> Seq.where(fun s -> 
                not <| String.IsNullOrEmpty s
            )

        // Gets the file info and then writes to said file.
        WriteToTextFile 
        <| SaveToFileInfo(sender) 
        <| actors

    // On creation of the object, creates reactive commands.
    do
        _GetActorsCommand <- ReactiveCommand.Create<Window>(WriteActorsToFile)

    // Getters and setters.
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

        and set(name) =
            this.RaiseAndSetIfChanged(&_FileName, name) |> ignore

    member this.Message
        with get() = _Message

        and set(message) =
            this.RaiseAndSetIfChanged(&_Message, message) |> ignore

    member this.FileBodySummary
        with get() = _FileBodySummary

        and set(value) =
            this.RaiseAndSetIfChanged(&_FileBodySummary, value) |> ignore

    member this.GetActorsCommand
        with get() = _GetActorsCommand

        and set(value) =
            this.RaiseAndSetIfChanged(&_GetActorsCommand, value) |> ignore
            