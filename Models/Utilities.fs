namespace ScriptOverview.Models

open Avalonia.Controls
open Avalonia.Platform.Storage
open System.IO
open ScriptOverview.Models.FileFilters

module Utilities =
    let ReadFromFileInfo(sender: Window) =
        // Task that when ran, lets the user pick a file on their computer.
        let task = async {
            let topLevel = TopLevel.GetTopLevel sender

            let options = 
                FilePickerOpenOptions() 
                |> fun option -> 
                    option.Title <- "Select File"
                    option.AllowMultiple <- false
                    option.FileTypeFilter <- [Word] 
                                           @ [FilePickerFileTypes.Pdf]
                                           @ [FilePickerFileTypes.TextPlain]
                    
                    option
            
            return! topLevel.StorageProvider.OpenFilePickerAsync options |> Async.AwaitTask
        }

        // Runs the task and returns the user's chosen file if it exists, else null.
        task |> Async.RunSynchronously |> Seq.tryExactlyOne
    
    // Gets information on where the user wants their file stored.
    let SaveToFileInfo(sender: Window) =
        // Task that when ran, gets the file we'll be saving to.
        let task = async {
            let topLevel = TopLevel.GetTopLevel sender

            let options = 
                FilePickerSaveOptions()
                |> fun options -> 
                    options.Title <- "Save File To"
                    options.SuggestedFileName <- "Actors List.txt"

                    options

            return! topLevel.StorageProvider.SaveFilePickerAsync options |> Async.AwaitTask
        }
        
        // Runs the task and returns the user's chosen file if it exists, else null.
        task |> Async.RunSynchronously

    // Writes to the given file.
    let WriteToTextFile (file: IStorageFile) (text: string seq) =
        match file with
        | null -> 
            ()
        | _ ->
            File.WriteAllLines (file.Path.LocalPath, text)

    

