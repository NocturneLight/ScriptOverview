namespace ScriptOverview.Views

open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open ScriptOverview.ViewModels

type FileSelectView () as this = 
    inherit UserControl ()

    do this.InitializeComponent()

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    // Depending on the view we're switching to, gives any information
    // from FileSelectViewModel to the view model we're switching to.
    member this.OnUnload(sender: obj, args: RoutedEventArgs) =
        let view = 
            (this.Parent :?> ContentControl).Content 
            :?> ViewModelBase
        
        match view with
        | :? FileOverviewViewModel as vm -> 
            let context = (this.DataContext :?> FileSelectViewModel)

            vm.FileContents <- context.DocumentBody
            vm.FileName <- $"What would you like to do with {context.FileName}?"
        
        | _ -> 
            ()

    // Event that triggers on button click.
    //member this.OnButtonClick(sender: obj, args: RoutedEventArgs) = 
        // Task that when ran, lets the user pick a file on their computer.
        //let task = async {
        //    let topLevel = TopLevel.GetTopLevel(this)
        //    let options = FilePickerOpenOptions()
        //    options.Title <- "Select File"

        //    return! topLevel.StorageProvider.OpenFilePickerAsync(options) |> Async.AwaitTask
        //}

        // Runs the task and returns the user's chosen file if it exists, else null.
        //let file = 
        //    task 
        //    |> Async.RunSynchronously
        //    |> Seq.cast<IStorageFile>
        //    |> Seq.tryExactlyOne

        //let parentContext = this.Parent.DataContext :?> MainWindowViewModel

        // Gets the instance of the view we'll be switching to and setting the 
        // FileContents field there.
        //let nextViewIndex = (List.findIndex 
        //                        <| (fun p -> p = parentContext.View) 
        //                        <| parentContext.Pages) 
        //                        + 1
        
        //parentContext.Pages[nextViewIndex] :?> FileOverviewViewModel
        //    |> match file with
        //        | None -> fun _ -> 
        //            ()
        //        | Some(file) -> fun v -> 
                    // Assigns the document's body to FileContents in FileOverviewViewModel.
        //            v.FileContents <- 
        //                    WordprocessingDocument.Open(file.Path.LocalPath, false).MainDocumentPart.Document.Body
                    
                    // Switches the view to FileOverviewViewModel.
                    //parentContext.NextView()

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
