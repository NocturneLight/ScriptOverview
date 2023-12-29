namespace ScriptOverview.ViewModels

open ReactiveUI

[<AllowNullLiteral>]
type ViewModelBase() =
    inherit ReactiveObject()

    let mutable _IsNavigationVisible = false

    member this.IsNavigationVisible 
        with get() = _IsNavigationVisible

        and set(value) =
            this.RaiseAndSetIfChanged(&_IsNavigationVisible, value) |> ignore