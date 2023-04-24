namespace CbzCreatorGui.Models;

public class NameValue
{
    public NameValue(string name, object value)
    {
        Name = name;
        Value = value;
    }

    public object? Value { get; set; }
    public string? Name { get; set; }
}