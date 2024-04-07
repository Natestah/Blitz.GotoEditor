
# Blitz.GotoEditor

This project is a helper for Openning Any text Editor to a File/Line/Column.

Blitz.GotoEditor features:
* Listing builtin editors
* Expands Environment variables
* Built in Aliases that you can list in your UI

## Getting started
* todo:

### How to set up a new project that utilizes Blitz.Goto:
* Create an [empty console Appliction](https://docs.avaloniaui.net/docs/getting-started).
* Add a the [nuget reference](https://www.nuget.org/packages/Avalonia.AvaloniaEdit/#versions-body-tab) to the latest version:
`<PackageReference Include="Blitz.GotoEditor" Version="x.y.z.t" />`

* List Built in Editors
```.cs
var definitions = new GotoDefinitions();
foreach (var gotoEditor in  new GotoDefinitions().GetBuiltInEditors())
{
    Console.WriteLine(
        $"Title: {gotoEditor.Title} -> {Path.Combine(gotoEditor.ExecutableWorkingDirectory, gotoEditor.Executable)} {gotoEditor.Arguments}");
}
```

* Select Notepad++ from the list
```.cs
var firstEditor = definitions.GetBuiltInEditors().FirstOrDefault(def => def.Title == "Notepad++")!;
```
* Create a Goto Action from the Editor. Goto Action takes care of translating the tags and environment variables.
```.cs
var gotoAction = new GotoAction(firstEditor);
```

* Define what you want to go to.  CreateAndTestFile will create a temp file path that you can test Goto Functionality with.
```.cs
new GotoTestFile().CreateAndTestFile(out var myFileName,out var lineNumber, out var columnNumber );
```

* Create a directive for the goto action.
```.cs
var directive = new GotoDirective(myFileName, lineNumber, columnNumber);
```

* Execute goto
```.cs
gotoAction.ExecuteGoto(directive);
```


