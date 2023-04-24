using System.IO.Compression;
using CbzCreator.Models;
using CmdLineArgsParser;
using Newtonsoft.Json;

namespace CbzCreator;

public static class Program
{
    public static int Main(string[] args)
    {
        new Parser().ShowInfo(false);
        if (args.Length == 0) {
            new Parser().ShowUsage<Options>();
            return -1;
        }

        var options = new Parser().Parse<Options>(args, out var errors);
        if (errors.Count > 0) {
            Console.WriteLine("Errors:");
            foreach (var error in errors) {
                Console.WriteLine(error.Message);
            }
            return -1;
        }

        options.InputPath = Path.GetFullPath(options.InputPath!);
        options.OutputPath = Path.GetFullPath(options.OutputPath!);

        Console.WriteLine($"Input : {options.InputPath}");
        Console.WriteLine($"Output: {options.OutputPath}");
        Console.WriteLine($"Title : {options.Title}");
        Console.WriteLine();

        if (!Directory.Exists(options.InputPath)) {
            Console.WriteLine("Input path not found");
            return -1;
        }

        if (!Directory.Exists(options.OutputPath))
            Directory.CreateDirectory(options.OutputPath!);

        // Write info file
        var info = new Info()
        {
            Title = options.Title,
            Author = options.Author
        };
        using (var jsonStream = new FileStream(Path.Combine(options.OutputPath, "details.json"), FileMode.Create, FileAccess.Write)) {
            using (var sr = new StreamWriter(jsonStream)) {
                sr.Write(JsonConvert.SerializeObject(info, Formatting.Indented));
            }
        }

        // Build CBZs
        foreach (var dir in Directory.GetDirectories(options.InputPath!)) {
            if (dir is "." or "..")
                continue;

            var idx = dir.LastIndexOf(Path.DirectorySeparatorChar);
            if (idx >= 0) {
                var name = SanitizeFilename(dir.Substring(idx + 1));
                Compress(Path.Combine(options.OutputPath!, $"{options.Title} - {name}.cbz"), dir);
            }
        }
        return 0;
    }

    private static void Compress(string name, string path)
    {
        Console.WriteLine($"Creating {Path.GetFileName(name)} from {path}");
        using var stream = new FileStream(name, FileMode.Create, FileAccess.Write);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);
        foreach (var file in Directory.GetFiles(path)) {
            var entry = zip.CreateEntry(Path.GetFileName(file), CompressionLevel.SmallestSize);
            using var writer = entry.Open();
            using var inputStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var buffer = new byte[32768];
            int read;
            while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0) {
                writer.Write(buffer, 0, read);
            }
        }
    }

    private static string SanitizeFilename(string filename)
    {
        var reserved = new[]
        {
            "/",
            "\\", "?", "*",
            "<", ">", "\"", "|"
        };
        foreach (var c in reserved) {
            filename = filename.Replace(c, "_");
        }
        return filename;
    }
}