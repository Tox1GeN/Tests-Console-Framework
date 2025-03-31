namespace MiniTestRunner;

public class Printer
{
    // Format (Example):
    
    public static string PadStringWithChar(string str, char padChar)
    {
        // Calculate padding for symmetry visualization and center `str` in the middle.
        // Add padding with padChar from left and right side.
        int stringLength = str.Length;
        int totalPadding = 80 - stringLength;
        int leftPadding = totalPadding / 2;
        int rightPadding = totalPadding - leftPadding;
        return new string(padChar, leftPadding) + str + new string(padChar, rightPadding);
    }
    public static void PrintSummary(TestResult result, string? header = null)
    {
        // bottom border (also top, in case of null header)
        string border = new string('-', 80);
        
        // Calculate padding and create padded results
        // Example: "Test passed: 10 / 20 (50.00%)"
        // Example: "Test failed: 10 / 20 (50.00%)"
        string passedTestPadded = PadStringWithChar($"Test passed: {result.Passed} / {result.Total} ({(result.Total > 0 ? (result.Passed * 100.0 / result.Total) : 0):F2}%)", ' ');
        string failedTestPadded = PadStringWithChar($"Test failed: {result.Failed} / {result.Total} ({(result.Total > 0 ? (result.Failed * 100.0 / result.Total) : 0):F2}%)", ' ');
        
        // Replace last and first ' ' with '|' for borders;
        passedTestPadded = "|" + passedTestPadded.Substring(1, passedTestPadded.Length - 2) + "|";
        failedTestPadded = "|" + failedTestPadded.Substring(1, failedTestPadded.Length - 2) + "|";
        
        Console.WriteLine();
        
        // Print header and results
        if (header != null)
        {
            // Calculate padding and create padded header
            string headerPadded = PadStringWithChar(header, '-');
            Console.WriteLine(headerPadded);
            Console.WriteLine(passedTestPadded);
            Console.WriteLine(failedTestPadded);
            Console.WriteLine(border);
        }
        
        // Print results only
        else
        {
            Console.WriteLine(border);
            Console.WriteLine(passedTestPadded);
            Console.WriteLine(failedTestPadded);
            Console.WriteLine(border);
        }
        
        Console.WriteLine();
    }

    // Format: "[WARNING] message"
    public static void PrintWarning(string message)
    {
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("WARNING");
        Console.ResetColor();
        Console.Write($"] {message}\n");
    }
    
    // Format for a DataRow: "> [PASSED] message"
    public static void PrintPassed(string message)
    {
        Console.Write(" > [");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("PASSED");
        Console.ResetColor();
        Console.Write($"] {message}\n");
    }

    // Format for a single test: "[PASSED] test's name"
    public static void PrintSingleTestPassed(string testName)
    {
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("PASSED");
        Console.ResetColor();
        Console.Write($"] {testName}\n");
    }

    // Format: 
    // "[FAILED] test's name"
    // "REASON: reason"
    public static void PrintFailed(string testName, string reason)
    {
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("FAILED");
        Console.ResetColor();
        Console.WriteLine($"] {testName}");
        Console.ForegroundColor = ConsoleColor.Red;
        if (!string.IsNullOrEmpty(reason))
        {
            Console.WriteLine("REASON: " + reason);
        }
        Console.ResetColor();
    }
}