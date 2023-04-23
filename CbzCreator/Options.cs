using CmdLineArgsParser;
using CmdLineArgsParser.Attributes;

namespace CbzCreator;

public class Options : IOptions
{
    [Option("title", 't',
        Description = "The comic title",
        Required = true)]
    public string? Title { get; set; }

    [Option("input", 'i',
        Description = "The path containing all the comic books",
        Required = true)]
    public string? InputPath { get; set; }

    [Option("output", 'o',
        Description = "The path where the cbz files will be created",
        Required = true)]
    public string? OutputPath { get; set; }
}