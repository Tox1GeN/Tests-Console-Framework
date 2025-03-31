namespace MiniTest.Attributes;

[ AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DataRowAttribute : Attribute
{
    public object?[] Data { get; }
    public string? Description { get; set; }
    public DataRowAttribute(params object?[] data) => Data = data;
}