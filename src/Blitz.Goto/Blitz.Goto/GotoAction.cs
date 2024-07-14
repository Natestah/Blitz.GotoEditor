using System.Diagnostics;
using System.Text;

namespace Blitz.Goto;

public class GotoAction(GotoEditor gotoEditor)
{
    
    private bool LocateDirectoryFromSystemPath(string inputWorkingDirectory, string exeName, out string workingDirectory)
    {
        workingDirectory = null;
        
        if (!string.IsNullOrEmpty(inputWorkingDirectory))
        {
            return false;
        }

        string[]? pathVars;
        try
        {
            pathVars = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';', StringSplitOptions.TrimEntries);
        }
        catch
        {
            return false;
        }
        // search for it in %path% environment variable.
        foreach (var path in pathVars)
        {
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            try
            {
                var test = Path.Combine(path, exeName);
                if (!File.Exists(test))
                {
                    continue;
                }
            }
            catch (Exception ex)
            {
                continue;
            }
            workingDirectory = path;
            return true;
        }
        return false;
    }

    
    public void ExecuteGoto( GotoDirective gotoDirective)
    {
        if (!string.IsNullOrEmpty(gotoEditor.CodeExecute))
        {
            switch (gotoEditor.CodeExecute)
            {
                case "VisualStudioPlugin":
                    var appdata = Environment.ExpandEnvironmentVariables("%appdata%");
                    string path = Path.Combine(appdata, "NathanSilvers", "POORMANS_IPC");
                    Directory.CreateDirectory(path);
                    string file = Path.Combine(path, "VISUAL_STUDIO_GOTO.txt");
                    File.WriteAllText(file, $"{gotoDirective.FileName},{gotoDirective.Line},{gotoDirective.Column}");
                    break;
            }

            return;
        }
        var startInfo = GetStartinfoForDirective(gotoDirective);
        Process.Start(startInfo);
    }

    
    private bool CanGotoVisualStudio()
    {
        try
        {
            // search for it in %path% environment variable.
            foreach (var path in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';',
                         StringSplitOptions.TrimEntries))
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (path.Contains("Visual Studio"))
                {
                    return true;
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }
    
    public bool CanDoAction()
    {
        switch (gotoEditor.CodeExecute)
        {
            case "VisualStudioPlugin":
                return CanGotoVisualStudio();
        }
        return LocateExecutable(out _, out _);
    }

    private bool LocateExecutable(out string workingDirectory, out string fileName)
    {
        workingDirectory = Environment.ExpandEnvironmentVariables(gotoEditor.ExecutableWorkingDirectory);
        if (LocateDirectoryFromSystemPath(workingDirectory,gotoEditor.Executable, out var foundPath))
        {
            workingDirectory = foundPath;
        }

        if (GotoJetbrainsIDE.Instance.IsMatchForWorkingDirectory(workingDirectory)
            &&  GotoJetbrainsIDE.Instance.GetWorkingDirectory(workingDirectory, gotoEditor.Executable, out var matched))
        {
            workingDirectory = matched!;
        }
        
        fileName = Path.Combine(workingDirectory, gotoEditor.Executable);
        return File.Exists(fileName);
    }

    public ProcessStartInfo GetStartinfoForDirective(GotoDirective gotoDirective, bool forPreview = false)
    {
        if(!LocateExecutable(out var workingDirectory, out var fileName)&& !forPreview)
        {
            throw new FileNotFoundException("Goto Editor not found.", fileName);
        }
        
        return new ProcessStartInfo(fileName)
        {
            CreateNoWindow = true,
            WorkingDirectory =  workingDirectory,
            Arguments = new GotoArgumentConverter(gotoDirective).ConvertArguments(gotoEditor.Arguments)
        };
    }

}