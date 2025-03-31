namespace MiniTestRunner;

public class TestResult
{
    public int Total { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    
    public TestResult(int total, int passed, int failed) => (Total, Passed, Failed) = (total, passed, failed);
}