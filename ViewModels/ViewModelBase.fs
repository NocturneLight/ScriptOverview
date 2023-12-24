namespace ScriptOverview.ViewModels

open ReactiveUI

[<AbstractClass>]
type ViewModelBase() =
    inherit ReactiveObject()
    
    let mutable _View: ViewModelBase = Unchecked.defaultof<ViewModelBase>

    member this.View 
        with get() = _View
        
        and set(view) = 
            this.RaiseAndSetIfChanged(&_View, view) |> ignore