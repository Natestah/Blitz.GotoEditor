namespace Blitz.Goto;


/// <summary>
/// JetBrains Rider installs post a significant challenge to the simple list of Editors and their installs.
/// where path looks like 'C:\Program Files\jetbrains\JetBrains Rider 2023.3.3\bin' We simply need some code to discover the latest and greatest.
/// </summary>
public class GotoJetbrainsRider
{
    public static GotoJetbrainsRider Instance = new GotoJetbrainsRider();
    
    public bool IsMatchForWorkingDirectory(string workingDirectory) => workingDirectory == "%JETBRAINS_WORKING_DIR%";

    private string? _foundDirectory = null;
    public bool GetWorkingDirectory(out string workingDirectory)
    {
        if (_foundDirectory != null)
        {
            workingDirectory = _foundDirectory;
            return true;
        }
        string path = Environment.ExpandEnvironmentVariables(@"%programfiles%\jetbrains");
        DateTime latestAndGreatest = DateTime.MinValue;
        workingDirectory = null;
        foreach (var subDirectory in Directory.EnumerateDirectories(path).Where((p)=>p.Contains(@"\JetBrains Rider ") ))
        {
            string searchDirectory = Path.Combine(subDirectory, "bin"); 
            string searchPath = Path.Combine(searchDirectory, "rider64.exe");
            if (File.Exists(searchPath))
            {
                var age = File.GetLastWriteTimeUtc(searchPath);
                if (age > latestAndGreatest)
                {
                    workingDirectory = searchDirectory;
                    latestAndGreatest = age;
                }
            }
        }

        _foundDirectory = workingDirectory;
        return workingDirectory != null;
    }
}