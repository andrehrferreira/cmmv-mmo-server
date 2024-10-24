using System.Diagnostics;

public class UnrealUtils
{
    public static void RegenerateVSCodes(string clientPath, string unrealEnginePath, string projectName)
    {
        if (string.IsNullOrWhiteSpace(clientPath) || string.IsNullOrWhiteSpace(unrealEnginePath))
            throw new Exception("Invalid configuration. Verify that the paths are correct in the .env file.");

        try
        {
            Console.WriteLine();
            Console.WriteLine("Regeneration of client Visual Studio files...");

            string dotnetExe = Path.Combine("C:\\Program Files\\dotnet", "dotnet.exe");

            if (!File.Exists(dotnetExe))
                throw new Exception($"Dotnet executable not found at path: {dotnetExe}");

            string unrealBuildToolPath = Path.Combine(unrealEnginePath, "Build", "BatchFiles", "Build.bat");

            if (!File.Exists(unrealBuildToolPath))
                throw new Exception($"UnrealBuildTool.dll not found at path: {unrealBuildToolPath}");

            string projectFilePath = Path.Combine(clientPath, $"{projectName}.uproject");

            if (!File.Exists(projectFilePath))
                throw new Exception($"Unreal project file not found at path: {projectFilePath}");

            string arguments = $"-Target=\"ClientEditor Win64 Development -Project=\"\"{projectFilePath}\"\"\" -WaitMutex -FromMsBuild";

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = unrealBuildToolPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = clientPath
            };

            using (Process process = Process.Start(processInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);

                process.WaitForExit();

                if (process.ExitCode == 0)
                    Console.WriteLine("Regeneration of client Visual Studio files completed successfully.");
                else
                    throw new Exception($"Error regenerating files. Exit code: {process.ExitCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error regenerating files: {ex.Message}");
        }
    }
}
