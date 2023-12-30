namespace ScriptOverview.Models

open Avalonia.Controls
open Avalonia.Platform.Storage
open System.IO

module Utilities =
    // Gets information on where the user wants their file stored.
    let SaveToFileInfo(sender: Window) =
        // Task that when ran, gets the file we'll be saving to.
        let task = async {
            let topLevel = TopLevel.GetTopLevel sender

            let options = 
                FilePickerSaveOptions()
                |> fun options -> 
                    options.Title <- "Save File To"
                    options.SuggestedFileName <- "Actors.txt"

                    options

            return! topLevel.StorageProvider.SaveFilePickerAsync options |> Async.AwaitTask
        }
        
        // Executes the task.
        task |> Async.RunSynchronously

    // Writes to the given file.
    let WriteToTextFile (file: IStorageFile) (text: string seq) =
        match file with
        | null -> 
            ()
        | _ ->
            File.WriteAllLines (file.Path.LocalPath, text)

