namespace ScriptOverview.Models

open Avalonia.Platform.Storage

module FileFilters =
    // Creates a filter for Word documents.
    let Word = 
        FilePickerFileType "Word Document"
        |> fun (file) -> 
            file.Patterns <- ["*.docx"] @ ["*.doc"]
            file.MimeTypes <- ["officedocument.wordprocessingml.document"]

            file

