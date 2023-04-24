using System.IO.Compression;
using System.Security.Cryptography;
using CbzCreator.Lib.Models;
using Newtonsoft.Json;

namespace CbzCreator.Lib;

public class Creator
{
    /// <summary>
    /// MD5 of images to skip (ad from source sites)
    /// </summary>
    private static readonly HashSet<string> Md5ToSkip = new()
    {
        "a15f2b2e0ebd6bdda7c338135caa8398" // mangafox "more wonderful manga here"
    };

    /// <summary>
    /// Create the CBZ files
    /// </summary>
    /// <param name="info">The comic <see cref="Info"/></param>
    /// <param name="inputPath">The input path</param>
    /// <param name="outputPath">The output path</param>
    public static void Create(Info info, string inputPath, string outputPath, Action<string>? logger = null)
    {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath!);

        // Write info file
        using (var jsonStream = new FileStream(Path.Combine(outputPath, "details.json"), FileMode.Create, FileAccess.Write)) {
            using (var sr = new StreamWriter(jsonStream)) {
                sr.Write(JsonConvert.SerializeObject(info, Formatting.Indented));
            }
        }

        // Build CBZs
        var title = SanitizeFilename(info.Title!);
        foreach (var dir in Directory.GetDirectories(inputPath)) {
            if (dir is "." or "..")
                continue;

            var idx = dir.LastIndexOf(Path.DirectorySeparatorChar);
            if (idx >= 0) {
                var name = SanitizeFilename(dir.Substring(idx + 1));
                var output = Path.Combine(outputPath!, $"{title} - {name}.cbz");
                logger?.Invoke($"Creating {Path.GetFileName(output)} from {dir}");

                Compress(output, dir);
            }
        }
    }

    /// <summary>
    /// Compress a folder into a single CBZ file
    /// </summary>
    /// <param name="outputPath">The output file path</param>
    /// <param name="inputPath">The input folder containing images</param>
    private static void Compress(string outputPath, string inputPath)
    {
        using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);
        foreach (var file in Directory.GetFiles(inputPath)) {
            var md5 = CalculateMd5(file);
            if (Md5ToSkip.Contains(md5))
                continue;

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

    /// <summary>
    /// Remove invalid characters from a file name
    /// </summary>
    /// <param name="filename">The file name</param>
    /// <returns></returns>
    private static string SanitizeFilename(string filename)
    {
        var reserved = new[]
        {
            "/",
            "\\", "?", "*", ":",
            "<", ">", "\"", "|",
        };
        foreach (var c in reserved) {
            filename = filename.Replace(c, "_");
        }
        return filename;
    }

    /// <summary>
    /// Calculate the MD5 hash of a file
    /// </summary>
    /// <param name="filePath">The file path</param>
    /// <returns></returns>
    private static string CalculateMd5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}