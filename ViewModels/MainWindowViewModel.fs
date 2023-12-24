namespace ScriptOverview.ViewModels

open System.Collections
open ReactiveUI
open System.Collections.Generic
open System.Windows.Input
open System.Diagnostics

type MainWindowViewModel() as this =
    inherit ViewModelBase()
    
    // A private list of all possible views for this program.
    let _Pages: ViewModelBase list = [
        new FileSelectViewModel()
        new FileOverviewViewModel()
    ]

    do this.View <- _Pages[0] // Sets the starting view.

    member this.Pages with get() = _Pages

    // Creates a truth condition for determining if a view is in the list,
    // then pipes the list to the truth condition and then pipes that to findIndex.
    // The number returned + 1 is our next view to switch to.
    member this.NextView() = 
        let nextIndex = (List.findIndex <| (fun p -> p = this.View) <| _Pages) + 1
        
        this.View <- this.Pages[nextIndex]


    
