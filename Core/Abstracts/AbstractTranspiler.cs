using System.Reflection;

public abstract class AbstractTranspiler
{
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

    public static IEnumerable<Type> GetContracts()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var contracts = types.Where(t => t.IsValueType && t.GetCustomAttribute<ContractAttribute>() != null);
        return contracts;
    }
}
