namespace ScriptOverview.ViewModels

open ReactiveUI
open System.Windows.Input
open System

type MainWindowViewModel() as this =
    inherit ViewModelBase()
    
    // A private list of all possible views for this program.
    let _Pages: PageViewModelBase option list = [
        option.Some (FileSelectViewModel())
        option.Some (FileOverviewViewModel())
    ]
    
    static let mutable _View: PageViewModelBase = null
    let mutable _NextViewCommand: ICommand = null
    let mutable _PrevViewCommand: ICommand = null

    // Sets the view to the next view.
    let GoToNextView() =
        let index = List.tryFindIndex(fun (v: PageViewModelBase option) -> v.Value = this.View) <| this.Pages

        match index with
        | Some(i) -> 
            let nextIndex = i + 1
            let item = List.tryItem nextIndex this.Pages

            match item with
            | Some(view) ->
                this.View <- view.Value
            
            | None -> 
                ()

        | None -> 
            raise (ArgumentNullException("View was set to an object not in the Pages list."))

    // Sets the view to the previous view.
    let GoToPrevView() =
        let index = List.tryFindIndex(fun (v: PageViewModelBase option) -> v.Value = this.View) <| this.Pages

        match index with
        | Some(i) ->
            let prevIndex = i - 1
            let item = List.tryItem prevIndex this.Pages

            match item with
            | Some(view) ->
                this.View <- view.Value
            | None ->
                ()
        
        | None -> 
            raise (ArgumentNullException("View was set to an object not in the Pages list."))

    // On creation, we set the view and create Reactive commands for going
    // to the next and previous view.
    do
        _View <- (List.head _Pages).Value
        
        // We create observables that disable or enable the next and previous buttons
        // depending on the truth values of CanNavigateNext and CanNavigatePrevious
        // on the current view.
        let canNavigateNext = this.WhenAnyValue(fun x -> x.View.CanNavigateNext)
        let canNavigatePrev = this.WhenAnyValue(fun x -> x.View.CanNavigatePrevious)

        _NextViewCommand <- ReactiveCommand.Create(GoToNextView, canNavigateNext)
        _PrevViewCommand <- ReactiveCommand.Create(GoToPrevView, canNavigatePrev)

    // Getters and setters.
    member this.View
        with get(): PageViewModelBase = _View

        and set(view) = 
            this.RaiseAndSetIfChanged(&_View, view) |> ignore

    member this.Pages 
        with get(): PageViewModelBase option list = _Pages

    member this.NextViewCommand 
        with get() = _NextViewCommand

    member this.PrevViewCommand
        with get() = _PrevViewCommand



    
