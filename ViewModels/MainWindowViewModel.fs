namespace ScriptOverview.ViewModels

open ReactiveUI
open System
open Avalonia.Platform.Storage
open NLog

type MainWindowViewModel() =
    inherit WindowViewModelBase()
    
    let _Log = LogManager.GetCurrentClassLogger()

    // All possible views for the program to switch between
    // with an enum value serving as the key.
    let _ViewDictionary = 
        dict<VIEWS, ViewModelBase> [
            FILESELECT, FileSelectViewModel()
            FILEOVERVIEW, FileOverviewViewModel()
        ]
    
    // The current view.
    let mutable _View: ViewModelBase option = None

    // On creation, we set the view to file select.
    // This is our starting view.
    do _View <- _ViewDictionary.Item FILESELECT |> Some

    override this.GoToView(view: VIEWS, [<ParamArrayAttribute>] args: 'a array) =
        try
            match view with
            | FILEOVERVIEW ->
                // Cast the generic type to IStorageFile.
                let file = box (Array.head args) :?> IStorageFile
            
                // Then sends the IStorageFile to File Overview
                // to set up the next view.
                _ViewDictionary.Item view :?> FileOverviewViewModel
                |> fun vm ->
                    vm.InitializeView(file)

            | _ ->
                ()
        with
        | ex ->
            _Log.Error(ex, "An error occurred while trying to switch views.")
            
        // Switch to the given view.
        this.View <- Some <| _ViewDictionary.Item view
    
    // Getters and setters.
    member this.View
        with get() = _View

        and set(view) = 
            this.RaiseAndSetIfChanged(&_View, view) |> ignore

    member this.Pages 
        with get() = _ViewDictionary



    
