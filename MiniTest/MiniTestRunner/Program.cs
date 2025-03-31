using System;
using System.Linq;
using System.Runtime.Loader;

namespace MiniTestRunner;

class Program
{
    static int Main(string[] args)
    {
        // Check if any arguments were provided
        if (args.Length == 0)
        {
            Console.WriteLine("No assemblies provided. Usage: MiniTestRunner path_to_assembly.dll");
            return 1;
        }
        
        TestResult globalResult = new TestResult(0, 0, 0);

        // Loop through all the assemblies
        foreach (var assemblyPath in args)
        {
            // Load the assembly and run the tests for it
            TestResult resultAssembly = TestRunner.TestRunAssembly(assemblyPath);
            
            // Count the results
            globalResult.Total += resultAssembly.Total;
            globalResult.Passed += resultAssembly.Passed;
            globalResult.Failed += resultAssembly.Failed;
        }

        // Global summary for all assemblies (.dll files)
        Printer.PrintSummary(globalResult, "ForAllAssemblies");
        return globalResult.Failed > 0 ? 1 : 0;
    }
}