using System.Diagnostics;

namespace Blitz.Goto;

public class GotoAction(GotoEditor gotoEditor)
{
    private bool LocateDirectoryFromSystemPath(string inputWorkingDirectory, out string workingDirectory)
    {
        workingDirectory = null;
        if (!string.IsNullOrEmpty(inputWorkingDirectory))
        {
            return false;
        }
        // search for it in %path% environment variable.
        foreach (var path in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';', StringSplitOptions.TrimEntries))
        {
            if (string.IsNullOrEmpty(path))
            {
                continue;
            }
            var test = Path.Combine(path, gotoEditor.Executable);
            if (!File.Exists(test))
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
        var startInfo = GetStartinfoForDirective(gotoDirective);
        Process.Start(startInfo);
    }

    public ProcessStartInfo GetStartinfoForDirective(GotoDirective gotoDirective, bool forPreview = false)
    {
        var argumentConverter = new GotoArgumentConverter(gotoDirective);
        string workingDirectory = Environment.ExpandEnvironmentVariables(gotoEditor.ExecutableWorkingDirectory);

        if (LocateDirectoryFromSystemPath(workingDirectory, out var foundPath))
        {
            workingDirectory = foundPath;
        }

        if (GotoJetbrainsRider.Instance.IsMatchForWorkingDirectory(workingDirectory)
            &&  GotoJetbrainsRider.Instance.GetWorkingDirectory(out var matched))
        {
            workingDirectory = matched;
        }
        
        var fileName = Path.Combine(workingDirectory, gotoEditor.Executable);
        if (!File.Exists(fileName) && !forPreview)
        {
            throw new FileNotFoundException("Goto Editor not found.", fileName);
        }
        
        return new ProcessStartInfo(fileName)
        {
            CreateNoWindow = true,
            WorkingDirectory =  workingDirectory,
            Arguments = argumentConverter.ConvertArguments(gotoEditor.Arguments)
        };
    }

}