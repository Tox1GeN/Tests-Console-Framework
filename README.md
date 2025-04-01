# Short Guide & Explanation
---

## Project Launch
I launch `MiniTestRunner.csproj` using `dotnet` in main repository directory:
```bash
cd MiniTest
dotnet build
dotnet run --project MiniTestRunner/MiniTestRunner.csproj -- AuthenticationService.Tests/bin/Debug/net8.0/AuthenticationService.Tests.dll
```

## Logic

### First Component - MiniTest
Provides essential testing functionality, including:
- **Attributes**: Define test structure and behavior.
  - `TestClassAttribute`: Marks a class as a test class.
  - `TestMethodAttribute`: Marks a method as a test method.
  - `BeforeEachAttribute` and `AfterEachAttribute`: Indicate setup and teardown methods for tests.
  - `PriorityAttribute`: Assigns priority to test methods.
  - `DataRowAttribute`: Enables parameterized tests with data sets.
  - `DescriptionAttribute`: Adds descriptions to classes and methods.
- **Assertions**: Validate test outcomes.
  - `IsTrue` and `IsFalse`: Validate conditions.
  - `AreEqual` and `AreNotEqual`: Compare expected and actual values.
  - `ThrowsException`: Ensures expected exceptions are thrown.
  - `Fail`: Explicitly fails a test.
- **Exceptions**: `AssertException` is used to report test failures.

### Second Component - MiniTestRunner
#### Program
Loads provided assemblies by using `LoadAssembly()` and launch for each `TestRunAssembly()` from `TestRunner.cs`.  
Runs `Printer.PrintSummary()` for all assemblies.

#### AssemblyLoader
AssemblyLoader.cs` is a class only with one method `LoadAssembly()`. It loads provided assembly and also its dependencies in current directory.

#### TestRunner
Runs all the tests, counts the results:

#####  - TestRunAssembly
  For each class from provided assembly runs `TestRunClass()`.
  Runs `Printer.PrintSummary()` for provided assembly.

##### - TestRunClass
For each method from provided class runs `LaunchTest()`.
Runs `Printer.PrintSummary()` for provided class.

##### - LaunchTest
Checks if dataRows attribute was provided. In positive case `try` to invoke test method for each dataRow.  
Otherwise `try` only single test for provided data. But before each test `try` to invoke beforeEach method.  
If beforeEach and test method runs without any problem test will be counted as passed.

#### Printer
Displays important information about warnings, failed and passed tests. Prints test summary for specific module.  
##### - PadStringWithChar
Is needed to print symmetry table with results in `PrintSummary`.
##### - PrintSummary
Prints results for provided module.  
##### - PrintPassed
Prints, that tests passed for group of dataRows.  
##### - PrintSingleTestPassed
Prints, that single test passed.  
##### - PrintWarning
Prints detailed warning for provided test.  
##### - PrintFailed
Prints, that provided test failed and the reason of fail.
  
**TestResult:** Object of this class stores the passed, failed and total count of tests.

### References in Project
 - In `MiniTestRunner.csproj`:
 ```xml
 <ItemGroup>
    <ProjectReference Include="..\MiniTest\MiniTest.csproj" />
  </ItemGroup>
  ```
  - In `AuthenticationService.Tests.csproj`:
 ```xml
 <ItemGroup>
    <ProjectReference Include="..\AuthenticationService\AuthenticationService.csproj" />
    <ProjectReference Include="..\MiniTest\MiniTest.csproj" />
</ItemGroup>
  ```
