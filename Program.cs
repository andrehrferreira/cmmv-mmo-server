using System.Reflection;
using System.Runtime.InteropServices;
using DotNetEnv;

class Program
{
    static void Main()
    {
        try
        {
            string projectDirectory = GetProjectDirectory();
            string envFile = Path.Combine(projectDirectory, ".env");
            string sharedPath = Path.Combine(projectDirectory, "Shared");

            if (!File.Exists(envFile))            
                throw new FileNotFoundException($"The .env file was not found in the path: {envFile}");

            if(!Directory.Exists(sharedPath))
                throw new FileNotFoundException($"Please create the virtual link with the client first.");

            Env.Load(envFile);

            var clientProjectName = Env.GetString("CLIENT_PROJECTNAME");
            var clientPath = Env.GetString("CLIENT_PATH");
            var unrealEditorPath = Env.GetString("UNREAL_EDITOR_PATH");

            if (clientProjectName == "")                
                throw new Exception("The CLIENT_PROJECTNAME variable was not found in the .env file.");

            if (clientPath == "")                
                throw new Exception("The CLIENT_PATH variable was not found in the .env file.");

            if (unrealEditorPath == "")
                throw new Exception("The UNREAL_EDITOR_PATH variable was not found in the .env file.");

            ContractTraspiler.Generate();
            UnrealTraspiler.Generate(clientProjectName);
            UnrealUtils.RegenerateVSCodes(clientPath, unrealEditorPath, clientProjectName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }

        Console.ReadLine();
    }

    protected static string GetProjectDirectory()
    {
        string assemblyPath = Assembly.GetExecutingAssembly().Location;

        var directoryInfo = new DirectoryInfo(assemblyPath);

        while (directoryInfo != null && directoryInfo.Name != "bin")
            directoryInfo = directoryInfo.Parent;

        if (directoryInfo != null && directoryInfo.Parent != null)
            return directoryInfo.Parent.FullName;

        return AppDomain.CurrentDomain.BaseDirectory;
    }
}
