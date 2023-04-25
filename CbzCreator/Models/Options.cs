using CmdLineArgsParser;
using CmdLineArgsParser.Attributes;

namespace CbzCreator.Models;

public class Options : IOptions
{
    [Option("author", 'a',
        Description = "The author",
        Required = false)]
    public string? Author { get; set; }

    [Option("artist", 'r',
        Description = "The artist",
        Required = false)]
    public string? Artist { get; set; }

    [Option("title", 't',
        Description = "The title",
        Required = true)]
    public string? Title { get; set; }

    [Option("description", 'd',
        Description = "The description",
        Required = false)]
    public string? Description { get; set; }

    [Option("genre", 'g',
        Description = "The genre",
        Required = false)]
    public List<string>? Genre { get; set; }

    [Option("coverurl", 'c',
        Description = "The cover url",
        Required = false)]
    public Uri? CoverUrl { get; set; }

    [Option("input", 'i',
        Description = "The path containing all the comic books",
        Required = true)]
    public string? InputPath { get; set; }

    [Option("output", 'o',
        Description = "The path where the cbz files will be created",
        Required = true)]
    public string? OutputPath { get; set; }
}