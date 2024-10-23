using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

public class TestUtils
{
    private readonly string _projectDirectory;

    public TestUtils()
    {
        _projectDirectory = GetProjectDirectory();
    }

    public void RunAllTests()
    {
        Console.WriteLine("Searching for test classes...");

        var assembly = Assembly.GetExecutingAssembly();
        var testClasses = FindTestClasses(assembly);

        int totalFiles = testClasses.Count;
        int totalTests = 0;
        int totalPass = 0;
        int totalError = 0;

        var startTime = DateTime.Now;
        Stopwatch stopwatch = Stopwatch.StartNew();

        foreach (var testClass in testClasses)
        {
            try
            {
                totalTests++;
                Activator.CreateInstance(testClass);
                totalPass++;
            }
            catch (Exception ex)
            {
                totalError++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAIL] {testClass.Name}: {ex.Message}");
                Console.ResetColor();
            }
        }

        stopwatch.Stop();
        var duration = stopwatch.Elapsed;

        Console.WriteLine();

        if (totalError == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[PASS] All tests executed successfully.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[FAIL] {totalError} test(s) failed.");
        }

        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine($" Test Files  {totalFiles} passed ({totalPass})");
        Console.WriteLine($"      Tests  {totalTests} passed ({totalPass})");
        Console.WriteLine($"   Start at  {startTime:HH:mm:ss}");
        Console.WriteLine($"   Duration  {duration.TotalSeconds:F2}s (transform 1.60s, setup 0ms, collect 92.77s, tests {duration.TotalMilliseconds:F2}ms, environment 4ms, prepare 5.89s)");
        Console.ResetColor();
    }

    /// <summary>
    /// Finds all classes that inherit from AbstractTest in the given assembly.
    /// </summary>
    private List<Type> FindTestClasses(Assembly assembly)
    {
        return assembly.GetTypes()
                       .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AbstractTest)))
                       .ToList();
    }

    /// <summary>
    /// Gets the current project directory.
    /// </summary>
    private string GetProjectDirectory()
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
