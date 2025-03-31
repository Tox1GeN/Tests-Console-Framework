namespace MiniTest.Assertions;

public class AssertException : Exception
{
    public AssertException(string message) : base(message) { }
}