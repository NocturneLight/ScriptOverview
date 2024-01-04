namespace ScriptOverview.Models

module RegexFilters =
    let ScenePrologueRegex = 
        "(?<![^\s])((Scene|scene|Prologue|prologue)\s+[0-9]+|Prologue|prologue)(?![^\s])"

