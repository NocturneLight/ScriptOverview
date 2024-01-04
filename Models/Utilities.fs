namespace ScriptOverview.Models

open Avalonia.Controls
open Avalonia.Platform.Storage
open System.IO
open ScriptOverview.Models.FileFilters
open DocumentFormat.OpenXml
open DocumentFormat.OpenXml.Wordprocessing
open System.Text

module Utilities =
    // Gets information for the file the user wants read. 
    let GetReadFromFile(sender: Window) =
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
    let GetSaveToFile(sender: Window) (name: string) (fileExtension: string) =
        // Assembles a suggested file name based on the file name 
        // of the user's original chosen file. 
        let fileName = 
            name.Split "." 
            |> Array.head 
            |> StringBuilder
            |> fun sb -> sb.Append(fileExtension)
        
        // Task that when ran, gets the file we'll be saving to.
        let task = async {
            let topLevel = TopLevel.GetTopLevel sender

            let options = 
                FilePickerSaveOptions()
                |> fun options -> 
                    options.Title <- "Save File To"
                    options.SuggestedFileName <- fileName.ToString()

                    options

            return! topLevel.StorageProvider.SaveFilePickerAsync options |> Async.AwaitTask
        }
        
        // Runs the task and returns the user's chosen file if it exists, else null.
        task |> Async.RunSynchronously
    
    // Gets the folder to save files in.
    let GetFolder(sender: Window) =
        // Task that when ran, gets the folder to save files in.
        let task = async {
            let topLevel = TopLevel.GetTopLevel sender

            let options =
                FolderPickerOpenOptions()
                |> fun option ->
                    option.Title <- "Select Folder"
                    option.AllowMultiple <- false

                    option

            return! topLevel.StorageProvider.OpenFolderPickerAsync options |> Async.AwaitTask
        }

        // Runs the task and returns the user's chosen file if it exists, else null.
        task |> Async.RunSynchronously |> Seq.tryExactlyOne

    // Writes to the given file.
    let WriteToTextFile (file: string) (text: string seq) =
        match file with
        | null -> 
            ()
        | _ ->
            File.WriteAllLines (file, text)

    // Gets all instances of the Run object from a document's body paragraph by paragraph.
    let GetRunElementsFromDocument(body: OpenXmlElementList) = 
        body 
        |> Seq.map(fun e -> e.ChildElements 
                            |> Seq.where(fun e -> e :? Run) 
                            |> Seq.map(fun e -> e :?> Run)) 

    // Gets all the elements between a specified start index and a specified end index.
    let GetRangeOfSeqElements (sequence: string seq) (startIndex: int) (endIndex: int) =
        sequence
        |> Seq.skip startIndex
        |> Seq.take endIndex
        

    

