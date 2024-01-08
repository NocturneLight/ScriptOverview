namespace ScriptOverview.ViewModels

open ReactiveUI
open Avalonia.Controls.Documents
open DocumentFormat.OpenXml.Wordprocessing
open System.Windows.Input
open System
open Avalonia.Controls
open ScriptOverview.Models.Utilities
open System.Text.RegularExpressions
open System.Text
open ScriptOverview.Models.RegexFilters
open Avalonia.Platform.Storage
open ScriptOverview.Models.Types
open DocumentFormat.OpenXml.Packaging
open Avalonia.Media

type FileOverviewViewModel() =
    inherit ViewModelBase()
    
    let mutable _FileStructure: FileStructure option = None
    let mutable _Document: Body = null
    let mutable _FormattedDocumentContents: obj seq = Seq.empty
    let mutable _Message: TextBlock = null
    let mutable _GetActorsCommand: ICommand = null
    let mutable _SaveToNaniScriptCommand: ICommand = null
    let mutable _GoToViewCommand: ICommand = null

    // Returns the script formatted with text tags or for display.
    let FormatDocument (file: Run seq seq) (format: FORMAT) =
        file
        |> Seq.map(fun paragraph ->
            let sentence = FormatManager.GetFormat format
            
            // Sets the font if we're displaying to screen.
            match sentence with
            | :? TextBlock as block ->
                block.FontFamily <- FontFamily.Parse "Lucida"

            | _ ->
                ()

            paragraph
            // Iterates through every Run object and applies the appropriate tags 
            // to its inner text.
            |> Seq.iter(fun words ->
                let fragment = FormatManager.GetFormat format 
                
                // Sets the inner text to the text block or string builder.
                match fragment with
                | :? TextBlock as block ->
                    block.Text <- words.InnerText
                    
                | :? StringBuilder as builder ->
                    builder.Append words.InnerText |> ignore

                | _ ->
                    ()
                
                let matchCriteria = "[\S]+" // Apply text tags if the inner text is NOT just whitespace.

                match fragment with
                // Applies rich text settings to the text block object.
                | :? TextBlock as block ->
                    match words.RunProperties with
                    | null ->
                        ()

                    | _ as prop ->
                        // Applies italic tags to the text fragment.
                        match prop.Italic with
                        | _ when prop.Italic <> null && Regex.IsMatch(words.InnerText, matchCriteria) ->
                            block.FontStyle <- FontStyle.Italic

                        | _ ->
                            ()
                        
                        // Applies bold tags to the text fragment.
                        match prop.Bold with
                        | _ when prop.Bold <> null && Regex.IsMatch(words.InnerText, matchCriteria) ->
                            block.FontWeight <- FontWeight.Bold
                        
                        | _ ->
                            ()

                        // Applies color tags to the text fragment.
                        match prop.Color with
                        | _ when prop.Color <> null && Regex.IsMatch(words.InnerText, matchCriteria) ->
                            block.Foreground <- Brush.Parse $"#{prop.Color.Val}"    

                        | _ ->
                            ()

                        // Applies a font size to the text fragment.
                        match prop.FontSize with
                        | _ when prop.FontSize <> null && Regex.IsMatch(words.InnerText, matchCriteria) ->
                            block.FontSize <- float prop.FontSize.Val.Value
                 
                        | _ ->  
                            ()

                // Applies rich text settings to the string builder.
                | :? StringBuilder as builder ->
                    match words.RunProperties with
                    | null ->
                        ()
                    
                    | _ as prop ->
                        // Inserts a semicolon to make it a comment if the string
                        // is name of the scene we're on.
                        match Regex.IsMatch(words.InnerText, ScenePrologueRegex) with
                        | true ->
                            builder.Insert(0, "; ") |> ignore

                        | false ->
                            // Applies italic tags to the text fragment.
                            match prop.Italic with
                            | _ when prop.Italic <> null && Regex.IsMatch(words.InnerText, matchCriteria) ->
                                builder.Insert(0, "<i>") |> ignore
                                builder.Append "</i>" |> ignore

                            | _ ->
                                ()

                            // Applies bold tags to the text fragment.
                            match prop.Bold with
                            | _ when prop.Bold <> null && Regex.IsMatch(words.InnerText, matchCriteria) ->
                                builder.Insert(0, "<b>") |> ignore
                                builder.Append "</b>" |> ignore

                            | _ ->
                                ()

                            // Applies color tags to the text fragment.
                            match prop.Color with
                            | _ when prop.Color <> null && Regex.IsMatch(words.InnerText, matchCriteria) ->
                                builder.Insert(0, $"<color=#{prop.Color.Val}>") |> ignore
                                builder.Append "</color>" |> ignore

                            | _ ->
                                ()
                | _ ->
                    ()

                // Adds the sentence fragment to the textblock 
                // inline or string builder.
                match sentence with
                | :? TextBlock as block ->
                    block.Inlines.Add (fragment :?> TextBlock)

                | :? StringBuilder as builder ->
                    builder.Append fragment |> ignore

                | _ ->
                    ()
            )

            // Returns the textblock or string as an
            // object as that's the only way we can return it.
            match sentence with
            | :? TextBlock as block ->
                block :> obj

            | :? StringBuilder as builder ->
                builder.ToString() :> obj

            | _ ->
                ()
        )

    // Saves each scene to a .nani file of its own.
    let SaveToNaniScript(sender: Window) =
        // Gets the folder to write files to or brings back nothing.
        let folder = 
            GetFolder(sender) 
            |> Option.map(fun f -> f.TryGetLocalPath())
        
        // Writes the text to files if we get a folder, otherwise do nothing.
        match folder with
        | Some fldr ->
            // Adds text tags to the document.
            let formattedText = 
                FormatDocument 
                << GetRunElementsFromDocument <| _Document.ChildElements <| BUILDER
                |> Seq.cast<string>

            // Splits the string sequence on the start of a scene. 
            let textGroupings =
                formattedText 
                // Gets the index of every instance of the start of a scene.
                |> Seq.mapi(fun i s ->
                    match Regex.IsMatch(s, ScenePrologueRegex) with
                    | true ->
                        Some i
                    | false -> 
                        None
                )
                // Filters out every none value.
                |> Seq.where(fun i -> i <> None)
                // Gets the range of values between each start of a scene.
                |> fun sceneIndexes ->
                    sceneIndexes 
                    |> Seq.mapi(fun i sceneIndex -> 
                        let previousSceneIndex = sceneIndexes |> Seq.tryItem(i - 1)

                        match previousSceneIndex with
                        | Some idx ->
                            GetRangeOfSeqElements formattedText idx.Value (sceneIndex.Value - idx.Value)
                        | None ->
                            GetRangeOfSeqElements formattedText 0 (sceneIndex.Value - 0)
                    )

            // Gets the file info and writes each body of text to a file.
            textGroupings
            |> Seq.iter(fun group ->
                let fileName = Seq.head group

                match fileName with
                // When the file name is an empty string, then
                // it's the portion of the document with writer credits.
                | _ when fileName = String.Empty ->
                    WriteToTextFile (Some $"{fldr}/Credits.nani") group
                    
                // In all other cases, it's a regular scene, so we use the word "scene" and 
                // its number as the file name.
                | _ ->
                    // Gets the file name with the ; omitted.
                    let formattedName = Regex.Match(fileName, Regex.Match(fileName, "[^;]+").Value.Trim())
                    
                    WriteToTextFile (Some $"{fldr}/{formattedName.Value}.nani") group
            )
        | None ->
            ()

    // Function which parses the document for a
    // potential list of actors and writes them to a file.
    let WriteActorsToFile(sender: Window) =
        let actors = 
            _Document.ChildElements
            // Filters out anything that doesn't have a :.
            |> Seq.where(fun s -> 
                Regex.IsMatch(s.InnerText, "[A-Za-z'` ]+:"))
            // Filters out anything in the sentence that's after a :.
            |> Seq.map(fun s -> 
                Regex.Match(s.InnerText, "[A-Za-z'` ]+:").Value)
            // Filters out the : on all the remaining strings.
            |> Seq.map(fun s -> Regex.Match(s, "[A-Za-z'` ]+").Value)
            // Filters out anything that's longer than 3 words unless it contains the phrase 'at the same time'.
            |> Seq.where(fun s -> 
                Array.length <| Regex.Split(s, "\s") <= 3 || s.Trim().Contains "at the same time")
            // Filters out anything containing the words "Dream Sequence" or "PSA".
            |> Seq.where (fun s -> 
                not (s.Contains "Dream Sequence" || s.Contains "PSA"))
            // Removes any unecessary whitespace from the string.
            |> Seq.map(fun s -> s.Trim())
            // Removes any duplicate strings.
            |> Seq.distinct

        // Gets information on the user's chosen file or comes back with nothing.
        let filePath = 
            GetSaveToFile sender _FileStructure.Value.GetFileName " ~ Actor List.txt"
            |> Option.map(fun f -> f.TryGetLocalPath())

        // Writes to the user's chosen file or does nothing.
        WriteToTextFile filePath actors

    // Switches the view to the File Select view.
    let GoToView(sender: Window) =
        (sender.DataContext :?> WindowViewModelBase).GoToView FILESELECT

    // On creation of the object, creates reactive commands for each button.
    do
        _GetActorsCommand <- ReactiveCommand.Create<Window>(WriteActorsToFile)
        _SaveToNaniScriptCommand <- ReactiveCommand.Create<Window>(SaveToNaniScript)
        _GoToViewCommand <- ReactiveCommand.Create<Window>(GoToView)

    member this.InitializeView(file: IStorageFile) =
        // Gets and stores the file name and extension for later use.
        _FileStructure <- 
            FileStructure(Regex.Match(file.Name, FileNameRegex), Regex.Match(file.Name, FileExtensionRegex)) 
            |> Option.Some

        // Creates 3 TextBlocks forming a sentence. Each formatted differently.
        let messageBlock = 
            TextBlock() |> fun tb -> 
                tb.Text <- "What would you like to do with"; tb
        
        let nameBlock = 
            TextBlock() |> fun tb -> 
                tb.Text <- $" {_FileStructure.Value.GetFileName}"
                tb.Foreground <- Brush.Parse (Colors.SkyBlue.ToString())
                tb
        
        let questionBlock = TextBlock() |> fun tb -> tb.Text <- "?"; tb

        // Adds the 3 text boxes to the inline collection of the main TextBlock.
        let mainBlock = 
            TextBlock() |> fun mb -> 
                mb.Inlines.AddRange ([InlineUIContainer(messageBlock)] 
                                    @ [InlineUIContainer(nameBlock)] 
                                    @ [InlineUIContainer(questionBlock)])
                mb

        // Sets the message to display on the File Overview screen.
        this.Message <- mainBlock

        // Gets the contents of the document and then stores them in the code for later use.
        use document = WordprocessingDocument.Open(file.TryGetLocalPath(), false)
        
        _Document <- document.MainDocumentPart.Document.Body
        
        // Format the document with a seq of Runs pulled from the document and
        // sets the content panel.
        _FormattedDocumentContents <- FormatDocument << GetRunElementsFromDocument <| _Document.ChildElements <| BLOCK
        
    // Getters and setters.
    member val File: obj = null with get, set
    member val FileContents: Body = null with get, set
    
    member this.Message
        with get() = _Message

        and set(message) =
            this.RaiseAndSetIfChanged(&_Message, message) |> ignore

    member this.FormattedDocumentContents
        with get() = _FormattedDocumentContents

        and set(value) =
            this.RaiseAndSetIfChanged(&_FormattedDocumentContents, value) |> ignore

    member this.GetActorsCommand
        with get() = _GetActorsCommand

        and set(value) =
            this.RaiseAndSetIfChanged(&_GetActorsCommand, value) |> ignore

    member this.SaveToNaniScriptCommand
        with get() = _SaveToNaniScriptCommand

        and set(value) =
            this.RaiseAndSetIfChanged(&_SaveToNaniScriptCommand, value) |> ignore

    member this.GoToViewCommand
        with get() = _GoToViewCommand

        and set(value) =
            this.RaiseAndSetIfChanged(&_GoToViewCommand, value) |> ignore
            