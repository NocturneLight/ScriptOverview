namespace ScriptOverview.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open System
open ScriptOverview.ViewModels
open System.Collections


type FileOverviewView () as this = 
    inherit UserControl ()

    do this.InitializeComponent()

    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    
    member this.OnInitialization(sender: obj, args: EventArgs) =
        let context = this.DataContext :?> FileOverviewViewModel
        let fileContents = context.FileContents.ChildElements |> Seq.toList

        // TODO: Split list on empty spaces.
        //let s = List.partition (fun l -> l) <| context.FileContents.ChildElements |> List.toSeq
        printfn ""

    member this.GoToPreviousView(sender: obj, args: RoutedEventArgs) =
        printfn ""