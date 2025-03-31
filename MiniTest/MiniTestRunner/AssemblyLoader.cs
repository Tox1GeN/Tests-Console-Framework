using System.Reflection;
using System.Runtime.Loader;

namespace MiniTestRunner;

public class AssemblyLoader
{
    public static (Assembly assembly, AssemblyLoadContext alc) LoadAssembly(string assemblyPath)
    {
        string fullPath = Path.GetFullPath(assemblyPath);
        string assemblyDir = Path.GetDirectoryName(fullPath)!;
        
        var alc = new AssemblyLoadContext(null, isCollectible: true);
        
        // Look for dependencies in the same directory as the assembly. If they exist, load them.
        alc.Resolving += (context, name) =>
        {
            string depPath = Path.Combine(assemblyDir, name.Name + ".dll");
            if (File.Exists(depPath))
            {
                return context.LoadFromAssemblyPath(depPath);
            }
            return null;
        };
        
        // Load the assembly with the dependencies
        return (alc.LoadFromAssemblyPath(fullPath), alc);
    }
}