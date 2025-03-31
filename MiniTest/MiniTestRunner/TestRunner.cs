using System.Reflection;
using System.Runtime.Loader;
using MiniTest;
using MiniTest.Attributes;
using MiniTest.Assertions;

namespace MiniTestRunner;


public class TestRunner
{
    public static TestResult TestRunAssembly(string assemblyPath)
    {
        (var assembly, AssemblyLoadContext alc) = AssemblyLoader.LoadAssembly(assemblyPath);
        
        // Collect in List all Classes with [TestClass] attribute in loaded assembly
        var testClasses = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null)
            .ToList();

        TestResult resultAssembly = new TestResult(0, 0, 0);

        // Run tests for each class
        foreach (var testClass in testClasses)
        {
            TestResult resultClass = TestRunClass(testClass);
            
            resultAssembly.Total += resultClass.Total;
            resultAssembly.Passed += resultClass.Passed;
            resultAssembly.Failed += resultClass.Failed;
        }
        
        // Summary for Assembly
        Printer.PrintSummary(resultAssembly, Path.GetFileName(assemblyPath));
        
        // Memory free
        alc.Unload();
        
        return resultAssembly;
    }

    public static TestResult TestRunClass(Type testClass)
{
    // Print the description of the class before tests
    var classDescription = testClass.GetCustomAttribute<DescriptionAttribute>()?.Description;
    if (!string.IsNullOrEmpty(classDescription))
    {
        Console.WriteLine(classDescription);
    }
    
    Console.WriteLine($"Running tests from class {testClass.FullName}...");
    
    var constructorInfo = testClass.GetConstructor(Type.EmptyTypes);

    // If no parameterless constructor found, skip the class
    if (constructorInfo == null)
    {
        Printer.PrintWarning($"No parameterless constructor found for {testClass.FullName}. Tests in this class will be skipped.");
        return new TestResult(0, 0, 0);
    }

    object testClassInstance = Activator.CreateInstance(testClass)!;
    
    // Find methods with [BeforeEach] and [AfterEach] attributes
    var beforeEachMethod = testClass.GetMethods()
        .FirstOrDefault(m => m.GetCustomAttribute<BeforeEachAttribute>() != null);
    var afterEachMethod = testClass.GetMethods()
        .FirstOrDefault(m => m.GetCustomAttribute<AfterEachAttribute>() != null);
    
    // Collect in List all Methods with [TestMethod] attribute in current class
    // Then order it by Priority and Name (If no priority - set it to 0)
    // Priority can be set in [Priority] attribute
    var testMethods = testClass.GetMethods()
        .Where(m => m.GetCustomAttribute<TestMethodAttribute>() != null)
        .OrderBy(m => m.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0)
        .ThenBy(m => m.Name)
        .ToList();

    TestResult resultClass = new TestResult(0, 0, 0);

    // Run tests for each method
    foreach (var testMethod in testMethods)
    {
        // Try to collect DataRows from method
        var dataRows = testMethod.GetCustomAttributes<DataRowAttribute>().ToArray();
        
        // If no DataRows -> single test;
        if (dataRows.Length == 0)
        {
            resultClass.Total++;
            bool passed = LaunchTest(testClassInstance, beforeEachMethod, testMethod, afterEachMethod);
            if (passed) resultClass.Passed++; else resultClass.Failed++;
        }
        
        // Test with specified DataRows
        else
        {
            Console.WriteLine(testMethod.Name); // print method name before DataRows
            foreach (var dataRow in dataRows)
            {
                resultClass.Total++;
                bool passed = LaunchTest(testClassInstance, beforeEachMethod, testMethod, afterEachMethod, dataRow);
                if (passed) resultClass.Passed++; else resultClass.Failed++;
            }

            // Print the description of the method after all DataRows (if specified)
            var methodDescription = testMethod.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (!string.IsNullOrEmpty(methodDescription))
            {
                Console.WriteLine(methodDescription);
            }
        }
    }
    
    // Summary for current class
    Printer.PrintSummary(resultClass, testClass.FullName);
    return resultClass;
}


    private static bool LaunchTest(object testClassInstance, MethodInfo? beforeEach, MethodInfo testMethod,
        MethodInfo? afterEach, DataRowAttribute? dataRow = null)
    {
        string? methodDescription = testMethod.GetCustomAttribute<DescriptionAttribute>()?.Description;
        
        // if no parameters in method we can't pass null to `Invoke()`, but can array of empty objects
        var parameters = dataRow?.Data ?? Array.Empty<object>();
        
        // If no dataRow -> single test;
        var singleTest = (dataRow == null);
        
        // Check if count of parameters in method and in DataRow match. In case of mismatch - print warning and return false (failure).
        var methodParameters = testMethod.GetParameters();
        if (methodParameters.Length != parameters.Length)
        {
            Printer.PrintWarning($"Parameter mismatch in test {testMethod.DeclaringType?.FullName}{testMethod.Name}. Expected {methodParameters.Length} parameters, got {parameters.Length}.");
            Printer.PrintFailed(testMethod.Name, "Parameter mismatch.");
            return false;
        }
        
        // Launch beforeEach
        try
        {
            beforeEach?.Invoke(testClassInstance, null);
        }
        // In case of failure before each - return false (failure), because we can't even start test
        catch (Exception exception)
        {
            Printer.PrintFailed(testMethod.Name, $"BeforeEach method threw an exception: {exception.Message}");
            return false;
        }
        
        // Launch test
        try
        {
            testMethod.Invoke(testClassInstance, parameters);
            
            // Get the value of `Description` property of `DataDowAttribute class`
            string? description = dataRow?.Description;
            
            if (singleTest)
            {
                Printer.PrintSingleTestPassed(testMethod.Name);
                
                // Print the description of a single test (if specified)
                if (!string.IsNullOrEmpty(methodDescription))
                {
                    Console.WriteLine(methodDescription);
                }
            }
            else
            {
                Printer.PrintPassed(description ?? testMethod.Name);
            }
            return true;
        }
        catch (TargetInvocationException invokeInnerException)
        {
            Printer.PrintFailed(testMethod.Name,
                invokeInnerException.InnerException?.Message ?? invokeInnerException.Message);
            return false;
        }
        catch (Exception exception)
        {
            Printer.PrintFailed(testMethod.Name, exception.Message);
            return false;
        }
        finally
        {
            // Launch afterEach
            try
            {
                afterEach?.Invoke(testClassInstance, null);
            }
            // Print warning in case of failure afterEach. Don't commit failure, cause test is passed at this point
            catch (Exception exception)
            {
                Printer.PrintWarning($"AfterEach method threw an exception: {exception.Message}");
            }
        }
    }
}