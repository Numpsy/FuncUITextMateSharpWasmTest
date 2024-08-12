namespace TextMateSharpWasm

open Avalonia
open Avalonia.Browser
open Avalonia.Controls.ApplicationLifetimes

open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Experimental
open Avalonia.FuncUI.Types

open AvaloniaEdit
open AvaloniaEdit.Document
open AvaloniaEdit.TextMate
open TextMateSharp.Grammars

open System.Runtime.Versioning

type AvaloniaEdiComponent () =
    inherit StaticComponent ()

    [<Literal>]
    let SomeJson = """
    { 
        "Framework": "Avalonia.FuncUI",
        "Editor": "AvaloniaEdit",
        "Highlighting": "TextMateSharp"
    }
    """

    let registryOptions = RegistryOptions(ThemeName.TomorrowNightBlue)

    override this.Build () : IView =
        ViewBuilder.Create<AvaloniaEdit.TextEditor> [
            TextEditor.init (fun editor ->
                let installation = editor.InstallTextMate(registryOptions)
                 
                editor.Document <- TextDocument(SomeJson)

                registryOptions.GetScopeByLanguageId("json")
                |> installation.SetGrammar
            )
        ]

type App() =
    inherit Application()

    override this.Initialize() =
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? ISingleViewApplicationLifetime as single -> single.MainView <- AvaloniaEdiComponent()
        | _ -> ()

module public Program =

    [<assembly: SupportedOSPlatform("browser")>]
    do ()

    [<CompiledName "BuildAvaloniaApp">] 
    let public buildAvaloniaApp () = 
        AppBuilder
            .Configure<App>()

    [<EntryPoint>]
    let main argv =
        task {
            do! buildAvaloniaApp().StartBrowserAppAsync("out")
        } |> ignore
        0
