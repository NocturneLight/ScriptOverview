namespace ScriptOverview.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Interactivity
open Avalonia.Platform.Storage
open System.Collections
open System.IO
open DocumentFormat.OpenXml.Packaging
open System.Text.RegularExpressions
open ScriptOverview.ViewModels
open System.Diagnostics
open System

type MainWindow () as this = 
    inherit Window ()
    
    do this.InitializeComponent()

    member private this.InitializeComponent() =
#if DEBUG
        this.AttachDevTools()
#endif
        AvaloniaXamlLoader.Load(this)
