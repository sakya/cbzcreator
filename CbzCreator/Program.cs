﻿using CbzCreator.Lib;
using CbzCreator.Lib.Models;
using CbzCreator.Models;
using CmdLineArgsParser;

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

        var info = new Info()
        {
            Title = options.Title,
            Author = options.Author,
            Artist = options.Artist,
            Description = options.Description,
            Genre = options.Genre,
            CoverUrl = options.CoverUrl
        };
        Creator.Create(info, options.InputPath, options.OutputPath, null,
            (level, message) =>
            {
                if (level < Creator.LogLevel.Info)
                    return;

                var prefix = level switch
                {
                    Creator.LogLevel.Debug => "[DBG]",
                    Creator.LogLevel.Info => "[INF]",
                    Creator.LogLevel.Warning => "[WRN]",
                    Creator.LogLevel.Error => "[ERR]",
                    _ => string.Empty
                };
                Console.WriteLine($"{prefix}{message}");
            });

        return 0;
    }
}