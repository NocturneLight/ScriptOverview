namespace ScriptOverview.ViewModels

open System
open System.Windows.Input
open Avalonia.Platform.Storage

type VIEWS =
    | FILESELECT
    | FILEOVERVIEW

[<AbstractClass>]
type WindowViewModelBase() =
    inherit ViewModelBase()

    // Abstract signature for switching views.
    // NOTE: Either do "?args: 'a" or "[<ParamArrayAttribute>] args: 'a array"
    // Don't include the ? for the ParamArrayAttribute one, it causes errors.
    abstract GoToView: VIEWS * [<ParamArrayAttribute>] args: 'a array -> unit