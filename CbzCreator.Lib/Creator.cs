using System.IO.Compression;
using System.Security.Cryptography;
using CbzCreator.Lib.Models;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace CbzCreator.Lib;

public static class Creator
{
    private static readonly HttpClient HttpClient = new();

    /// <summary>
    /// MD5 of images to skip (ad from source sites)
    /// </summary>
    private static readonly HashSet<string> Md5ToSkip = new()
    {
        "a15f2b2e0ebd6bdda7c338135caa8398" // mangafox "more wonderful manga here"
    };

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Create the CBZ files
    /// </summary>
    /// <param name="info">The comic <see cref="Info"/></param>
    /// <param name="inputPath">The input path</param>
    /// <param name="outputPath">The output path</param>
    /// <param name="token">The <see cref="CancellationToken"/></param>
    /// <param name="logger">The logger function</param>
    public static void Create(Info info, string inputPath, string outputPath, CancellationToken? token = null, Action<LogLevel, string>? logger = null)
    {
        if (!Directory.Exists(inputPath))
            throw new Exception($"Cannot access path {inputPath}");
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        // Write info file
        logger?.Invoke(LogLevel.Info, $"Writing {Path.Combine(outputPath, "details.json")}");
        using (var jsonStream = new FileStream(Path.Combine(outputPath, "details.json"), FileMode.Create, FileAccess.Write)) {
            using (var sr = new StreamWriter(jsonStream)) {
                sr.Write(JsonConvert.SerializeObject(info, Formatting.Indented));
            }
        }

        // Download cover
        if (info.CoverUrl != null) {
            logger?.Invoke(LogLevel.Info,$"Downloading cover from {info.CoverUrl}");
            DownloadCover(info.CoverUrl, Path.Join(outputPath, "cover.jpg"));
            var imageBytes = HttpClient.GetByteArrayAsync(info.CoverUrl).Result;
            File.WriteAllBytesAsync(Path.Combine(outputPath, "cover.jpg"), imageBytes);
        }

        // Build CBZs
        var title = SanitizeFilename(info.Title!);
        var dirs = Directory.GetDirectories(inputPath);
        Array.Sort(dirs);
        foreach (var dir in dirs) {
            if (token?.IsCancellationRequested == true) {
                logger?.Invoke(LogLevel.Warning, "User aborted");
                return;
            }

            if (dir is "." or "..")
                continue;

            var idx = dir.LastIndexOf(Path.DirectorySeparatorChar);
            if (idx >= 0) {
                var name = SanitizeFilename(dir.Substring(idx + 1));
                var output = Path.Combine(outputPath, $"{title} - {name}.cbz");
                logger?.Invoke(LogLevel.Info, $"Creating {Path.GetFileName(output)} from {dir}");

                Compress(dir, output, token, logger);
            }
        }
    }

    /// <summary>
    /// Compress a folder into a single CBZ file
    /// </summary>
    /// <param name="inputPath">The input folder containing images</param>
    /// <param name="outputFile">The output file path</param>
    /// <param name="token">The <see cref="CancellationToken"/></param>
    /// <param name="logger">The logger function</param>
    private static void Compress(string inputPath, string outputFile, CancellationToken? token, Action<LogLevel, string>? logger = null)
    {
        using var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        var files = Directory.GetFiles(inputPath);
        Array.Sort(files);
        foreach (var file in files) {
            if (token?.IsCancellationRequested == true)
                return;

            logger?.Invoke(LogLevel.Debug, $"Processing {file}");
            var md5 = CalculateMd5(file);
            if (Md5ToSkip.Contains(md5)) {
                logger?.Invoke(LogLevel.Warning, $"Skipped file {file}");
                continue;
            }

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

    /// <summary>
    /// Download the cover and convert it to jpg format
    /// </summary>
    /// <param name="url">The cover url</param>
    /// <param name="filePath">The destination path</param>
    private static void DownloadCover(Uri url, string filePath)
    {
        var imageBytes = HttpClient.GetByteArrayAsync(url).Result;

        using var ms = new MemoryStream(imageBytes);
        using var img = Image.Load(ms);
        img.Save(filePath, new JpegEncoder()
        {
            Quality = 90
        });
    }
}