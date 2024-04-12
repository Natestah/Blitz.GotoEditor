using System.Diagnostics;

namespace Blitz.Goto;

public class GotoAction(GotoEditor gotoEditor)
{
    public void ExecuteGoto( GotoDirective gotoDirective)
    {
        var argumentConverter = new GotoArgumentConverter(gotoDirective);
        var workingDirectory = Environment.ExpandEnvironmentVariables(gotoEditor.ExecutableWorkingDirectory);
        if (string.IsNullOrEmpty(workingDirectory))
        {
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
                break;
            }
        }
        var fileName = Path.Combine(workingDirectory, gotoEditor.Executable);
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException("Goto Editor not found.", fileName);
        }
        var startInfo = new ProcessStartInfo(fileName)
        {
            CreateNoWindow = true,
            WorkingDirectory =  workingDirectory,
            Arguments = argumentConverter.ConvertArguments(gotoEditor.Arguments)
        };
        Process.Start(startInfo);
    }

}