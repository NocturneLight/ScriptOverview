namespace ScriptOverview.ViewModels

[<AbstractClass>]
[<AllowNullLiteral>]
type PageViewModelBase() =
    inherit ViewModelBase()

    abstract member CanNavigateNext: bool with get, set
    abstract member CanNavigatePrevious: bool with get, set

