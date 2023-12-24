namespace ScriptOverview.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open System.IO
open System.Collections
open Avalonia.Platform.Storage
open ScriptOverview.ViewModels
open DocumentFormat.OpenXml.Packaging

type FileSelectView () as this = 
    inherit UserControl ()

    do this.InitializeComponent()

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    // Event that triggers on button click.
    member this.OnButtonClick(sender: obj, args: RoutedEventArgs) = 
        // Task that when ran, lets the user pick a file on their computer.
        let task = async {
            let topLevel = TopLevel.GetTopLevel(this)
            let options = FilePickerOpenOptions()
            options.Title <- "Select File"

            return! topLevel.StorageProvider.OpenFilePickerAsync(options) |> Async.AwaitTask
        }

        // Runs the task and returns the user's chosen file, then
        // checks to confirm the file does indeed exist still.
        let file = 
            task 
            |> Async.RunSynchronously
            :> IEnumerable
            |> Seq.cast<IStorageFile>
            |> Seq.where(fun file -> File.Exists file.Path.LocalPath)
            |> Seq.exactlyOne

        let parentContext = this.Parent.DataContext :?> MainWindowViewModel

        // Gets the instance of the view we'll be switching to and setting the 
        // FileContents field there.
        //parentContext.GetNextViewInstance :> FileOverviewViewModel |> ignore
        let nextViewIndex = (List.findIndex <| (fun p -> p = parentContext.View) <| parentContext.Pages) + 1
        
        parentContext.Pages[nextViewIndex] 
            :?> FileOverviewViewModel 
            |> fun v -> 
                v.FileContents <- WordprocessingDocument.Open(file.Path.LocalPath, false).MainDocumentPart.Document.Body

        // Sets the FileContents member with the opened document's text.
        //this.DataContext 
        //    :?> FileSelectViewModel 
        //   |> fun vm -> vm.FileContents 
        //                    <- WordprocessingDocument.Open(file.Path.LocalPath, false).MainDocumentPart.Document.InnerText 

        // Goes to the FileOverviewView screen.
        parentContext
            |> fun vm -> vm.NextView()

        // TODO: Switch views to the FileOverview View.
        //this.VisualRoot :> main

        //this.DataContext :?> MainWindowViewModel |> fun x -> x.CharacterList
    //    let text = document.MainDocumentPart.Document.Body.InnerText 
    //            |> fun t -> Regex.Matches (t, "([A-Za-z'`]*[ ]?[A-Z][a-z]*):")
    //            |> List.ofSeq
    //            |> List.map (fun t -> Regex.Replace(t.Value, ":", ""))
    //            |> List.map (fun t -> t.Trim())
    //            |> List.distinct
    //            |> List.where (fun t -> 
    //                t.Length > 1 && 
    //                not (t.ToLower().Contains "sequence start") && 
    //                not (t.ToLower().Contains "eridu") &&
    //                not (t.ToLower().Contains "everyone") &&
    //                not (t.ToLower().Contains "psa"))
        
    //    let context = sender 
    //                :?> Button
    //                |> fun b -> b.DataContext :?> MainWindowViewModel

    //    context.Greeting <- "Greeting was changed."
    //    context.CharacterList <- text
        
    //    for item in text do
    //        Debug.WriteLine item

        //context.CurrentPage <- new DocumentOverviewViewModel()

    //    Debug.WriteLine (printfn "%A" [1; 2; 3])
