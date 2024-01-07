namespace ScriptOverview.Models

open System.Text.RegularExpressions
open System.Text
open Avalonia.Controls

module Types =
    type FileStructure(fileName: Match, fileExtension: Match) =
        let _Name = fileName
        let _Extension = fileExtension

        // Getters and setters.
        member this.GetFileName
            with get() = _Name.Value

        member this.GetExtension 
            with get() = _Extension.Value

        member this.GetFullFileName
            with get() = $"{this.GetFileName}{this.GetExtension}"

    type FORMAT =
        | BLOCK
        | BUILDER

    type FormatManager(text: string) =  
        static member GetFormat(format: FORMAT): obj =
            match format with
            | BLOCK ->
                TextBlock()
            | BUILDER ->
                StringBuilder()