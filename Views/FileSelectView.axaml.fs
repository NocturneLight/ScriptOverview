namespace ScriptOverview.Views

open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open ScriptOverview.ViewModels
open DocumentFormat.OpenXml

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
            let context = this.DataContext :?> FileSelectViewModel

            vm.FileContents <- context.DocumentBody
            vm.FileName <- context.FileName
            vm.Message <- $"What do you want to do with {context.FileName}?" 
            
            // Gets the raw string of each line in the document and puts it in a string sequence.
            vm.FileBodySummary <- Seq.map(fun (x: OpenXmlElement) -> x.InnerText) <| vm.FileContents.ChildElements

        | _ -> 
            ()
