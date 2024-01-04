namespace ScriptOverview.ViewModels

open ReactiveUI
open DocumentFormat.OpenXml.Wordprocessing
open System.Windows.Input
open System
open Avalonia.Controls
open ScriptOverview.Models.Utilities
open System.Text.RegularExpressions
open System.Text
open ScriptOverview.Models.RegexFilters

type FileOverviewViewModel() as this =
    inherit PageViewModelBase()
    
    let mutable _CanNavigateNext = false
    let mutable _CanNavigatePrevious = true
    let mutable _FileName = ""
    let mutable _Message = ""
    let mutable _FileBodySummary: string seq = Seq.empty
    let mutable _GetActorsCommand: ICommand = null
    let mutable _ConvertToNaniScriptCommand: ICommand = null

    // Function which parses the document for a
    // potential list of actors and writes them to a file.
    let WriteActorsToFile(sender: Window) =
        // Filters the document in various ways looking for potential names.
        let actors =
            this.FileBodySummary
            // Filters out anything the doesn't contain a ':'.
            |> Seq.where(fun (s: string) -> s.Contains ":")
            // Replaces any string that doesn't appear to be a name with the empty string. 
            |> Seq.map(fun s -> 
                let potentialActors = Array.head <| s.Split ":"

                match potentialActors with
                | name when name.Contains "," -> 
                    String.Empty

                | name when name.Contains "." ->
                    String.Empty

                | name when name.StartsWith "--" ->
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
            |> Seq.where(fun s -> not <| String.IsNullOrEmpty s)

        let filePath = (GetSaveToFile sender _FileName "~ Actor List.txt").Path.LocalPath

        // Gets the file info and then writes to said file.
        WriteToTextFile filePath actors

    let ConvertDocumentToNaniScript(sender: Window) =
        // Gets every instance of the Run object grouped by paragraph.
        let results = GetRunElementsFromDocument this.FileContents.ChildElements
        
        // The whole body of text, with text tags applied.
        let formattedText = 
            results
            // Looks for bold, italic, or colored text and adds text tags.
            |> Seq.map(fun para -> 
                let sentence = StringBuilder()

                para 
                // Iterates through every Run object and applies the appropriate tags 
                // to its inner text.
                |> Seq.iter(fun e -> 
                    let fragment = StringBuilder(e.InnerText)
                    let matchCriteria = "[\S]+" // Apply text tags if the inner text is NOT just whitespace.

                    match e.RunProperties with
                    | null ->
                        ()

                    // Inserts a semicolon to make it a comment if the string
                    // is name of the scene we're on.
                    | _ when Regex.IsMatch(e.InnerText, ScenePrologueRegex) ->
                        fragment.Insert(0, "; ") |> ignore

                    // Applies italic tags to the text fragment.
                    | prop when prop.Italic <> null && Regex.IsMatch(e.InnerText, matchCriteria) ->
                        fragment.Insert(0, "<i>") |> ignore
                        fragment.Append "</i>" |> ignore

                    // Applies bold tags to the text fragment.
                    | prop when prop.Bold <> null && Regex.IsMatch(e.InnerText, matchCriteria) ->
                        fragment.Insert(0, "<b>") |> ignore
                        fragment.Append "</b>" |> ignore

                    // Applies color tags to the text fragment.
                    | prop when prop.Color <> null && Regex.IsMatch(e.InnerText, matchCriteria) ->
                        fragment.Insert(0, $"<color=#{prop.Color.Val}>") |> ignore
                        fragment.Append "</color>" |> ignore

                    | _ ->
                        ()

                    // Adds the sentence fragment to the whole string we're making.
                    sentence.Append fragment |> ignore
                )

                sentence.ToString()
            )

        // A grouping of each text group that we want to have its own file.
        let bodies = 
            this.FileBodySummary 
            // Looks for the string that says "[Scene] or [Prologue] [some number]" or "[prologue]" using Regex.
            |> Seq.where(fun s -> 
                Regex.IsMatch(s, ScenePrologueRegex))
            // Looks for the index of every scene or prologue string.
            |> Seq.map(fun s -> 
                this.FileBodySummary |> Seq.findIndex((=) s))
            // Gets the range of elements between the previous index of a start scene 
            // and the index just before the next scene.
            |> fun breakpoint -> 
                breakpoint
                |> Seq.map(fun s ->
                    let currentIndex = breakpoint |> Seq.findIndex((=) s)
                    let previousElement = breakpoint |> Seq.tryItem (currentIndex - 1)

                    match previousElement with
                    | Some(value) ->
                        GetRangeOfSeqElements formattedText value (s - value)
                    | None ->
                        GetRangeOfSeqElements formattedText 0 s
                )
        
        // Gets the folder to write files to.
        let folder = GetFolder(sender)

        match folder with
        | Some(folder) ->
            // Gets the file info and writes each body of text to its own file.
            bodies
            |> Seq.iter(fun b ->
                let fileName = Seq.head b

                match fileName with
                // When the file name is an empty string, then
                // it's the portion of the document with writer credits.
                | _ when fileName = String.Empty ->
                    WriteToTextFile $"{folder.Path.LocalPath}/Credits.nani" b

                // In all other cases, it's a regular scene, so we use that scene
                // number string as the file name.
                | _ ->    
                    let formattedName = Regex.Match(fileName, "[^;]+").Value.Trim()

                    WriteToTextFile $"{folder.Path.LocalPath}/{formattedName}.nani" b
            )

        | None ->
            ()


    // On creation of the object, creates reactive commands.
    do
        _GetActorsCommand <- ReactiveCommand.Create<Window>(WriteActorsToFile)
        _ConvertToNaniScriptCommand <- ReactiveCommand.Create<Window>(ConvertDocumentToNaniScript)

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

    member this.ConvertToNaniScriptCommand
        with get() = _ConvertToNaniScriptCommand

        and set(value) =
            this.RaiseAndSetIfChanged(&_ConvertToNaniScriptCommand, value) |> ignore
            