using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Diagnostics;
using DotNetEnv;

class Program
{
    public static void Main(string[] args)
    {
        try
        {
            int port = int.Parse(args.Length > 1 ? args[1] : "8080");

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

            var process = Process.GetCurrentProcess();
            Console.WriteLine($"AES GCM: {AesGcm.IsSupported}");
            Console.WriteLine($"SSE42: {Sse42.IsSupported}");
            Console.WriteLine($"MinWorkingSet: {process.MinWorkingSet}");
            Console.WriteLine($"WorkingSet64: {process.WorkingSet64}");
            Console.WriteLine($"MaxWorkingSet: {process.MaxWorkingSet}");
            Console.WriteLine($"IsHardwareAccelerated (SIMD): {Vector.IsHardwareAccelerated}");
            Console.WriteLine();

#if DEBUG
            //System tests
            var testRunner = new TestUtils();
                var testPassed = testRunner.RunAllTests();

                if (testPassed)
                {
                    //Generate
                    ContractTraspiler.Generate();
                    UnrealTraspiler.Generate(clientProjectName);
                    UnrealUtils.RegenerateVSCodes(clientPath, unrealEditorPath, clientProjectName);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("As the application did not pass the tests, the contract files and files for Unreal were not generated..");
                }
            #else
                Console.WriteLine("Running in release mode. Tests and transpilers are skipped.");
            #endif
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
