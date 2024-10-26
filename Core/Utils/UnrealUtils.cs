/*
 * UnrealUtils
 * 
 * Author: Andre Ferreira
 * 
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 *    
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
            Console.WriteLine("Cleaning Intermediate directory...");

            string intermediatePath = Path.Combine(clientPath, "Intermediate");

            if (Directory.Exists(intermediatePath))
            {
                Directory.Delete(intermediatePath, true);
                Console.WriteLine("Intermediate directory cleaned successfully.");
            }
            else
            {
                Console.WriteLine("Intermediate directory does not exist or was already removed.");
            }

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
