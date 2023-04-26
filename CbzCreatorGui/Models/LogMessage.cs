using CbzCreator.Lib;

namespace CbzCreatorGui.Models;

public class LogMessage
{
    public Creator.LogLevel Level { get; set; }
    public string? Message { get; set; }
}