using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CbzCreator.Lib.Models;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace CbzCreator.Lib;

public static class Creator
{
    private static readonly HttpClient HttpClient = new();

    private sealed class NaturalComparer : IComparer<string> {

        private static int CompareChunks(string x, string y) {
            if (x[0] >= '0' && x[0] <= '9' && y[0] >= '0' && y[0] <= '9') {
                var tx = x.TrimStart('0');
                var ty = y.TrimStart('0');

                var result = tx.Length.CompareTo(ty.Length);

                if (result != 0)
                    return result;

                result = string.CompareOrdinal(tx, ty);

                if (result != 0)
                    return result;
            }

            return string.CompareOrdinal(x, y);
        }

        public int Compare(string? x, string? y) {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            if (y is null)
                return +1;

            var itemsX = Regex
                .Split(x, "([0-9]+)")
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();

            var itemsY = Regex
                .Split(y, "([0-9]+)")
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();

            for (var i = 0; i < Math.Min(itemsX.Count, itemsY.Count); ++i) {
                var result = CompareChunks(itemsX[i], itemsY[i]);

                if (result != 0)
                    return result;
            }

            return itemsX.Count.CompareTo(itemsY.Count);
        }
    }

    /// <summary>
    /// MD5 of images to skip (ad from source sites)
    /// </summary>
    private static readonly HashSet<string> Md5ToSkip = new()
    {
        "a15f2b2e0ebd6bdda7c338135caa8398", // mangafox "more wonderful manga here"
        "283f0603ddd6f00638cbaae993b4177c", // mangascreener
        "4a2ae7fbcc6bfe9c6a412caa19f6030b", // mangaproject.cbj.net
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
    /// <param name="totalProgress">The progress function</param>
    /// <param name="cbzProgress">The progress function for the single CBZ</param>
    public static void Create(Info info, string inputPath, string outputPath, CancellationToken? token = null,
        Action<LogLevel, string>? logger = null,
        Action<double>? totalProgress = null,
        Action<double>? cbzProgress = null)
    {
        if (!Directory.Exists(inputPath))
            throw new Exception($"Cannot access path {inputPath}");
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        // Write info file
        var detailsFile = Path.Combine(outputPath, "details.json");
        logger?.Invoke(LogLevel.Info, $"Writing {detailsFile}");
        if (File.Exists(detailsFile))
            logger?.Invoke(LogLevel.Warning, $"Overwriting {detailsFile}");
        using (var jsonStream = new FileStream(detailsFile, FileMode.Create, FileAccess.Write)) {
            using (var sr = new StreamWriter(jsonStream)) {
                sr.Write(JsonConvert.SerializeObject(info, Formatting.Indented));
            }
        }

        // Download cover
        if (info.CoverUrl != null) {
            logger?.Invoke(LogLevel.Info,$"Downloading cover from {info.CoverUrl}");
            try {
                DownloadCover(info.CoverUrl, Path.Join(outputPath, "cover.jpg"), logger);
            } catch (Exception ex) {
                logger?.Invoke(LogLevel.Warning, $"Failed to save cover: {ex.Message}");
            }
        }

        // Build CBZs
        var title = SanitizeFilename(info.Title!);
        var dirs = Directory.GetDirectories(inputPath);
        Array.Sort(dirs, new NaturalComparer());
        var idx = 0;
        foreach (var dir in dirs) {
            if (token?.IsCancellationRequested == true) {
                logger?.Invoke(LogLevel.Warning, "User aborted");
                return;
            }

            if (dir is "." or "..")
                continue;

            var name = SanitizeFilename(Path.GetFileName(dir));
            if (!string.IsNullOrEmpty(name)) {
                var output = Path.Combine(outputPath, $"{title} - {name}.cbz");
                logger?.Invoke(LogLevel.Info, $"Creating {Path.GetFileName(output)} from {dir}");

                Compress(dir, output, token, logger, cbzProgress);
            } else {
                logger?.Invoke(LogLevel.Warning, $"Skipped {dir}");
            }

            totalProgress?.Invoke((double)++idx / dirs.Length * 100.0);
        }
        logger?.Invoke(LogLevel.Info, "Done");
    }

    /// <summary>
    /// Compress a folder into a single CBZ file
    /// </summary>
    /// <param name="inputPath">The input folder containing images</param>
    /// <param name="outputFile">The output file path</param>
    /// <param name="token">The <see cref="CancellationToken"/></param>
    /// <param name="logger">The logger function</param>
    /// <param name="cbzProgress">The progress function for the single CBZ</param>
    private static void Compress(string inputPath, string outputFile, CancellationToken? token,
        Action<LogLevel, string>? logger = null,
        Action<double>? cbzProgress = null)
    {
        if (File.Exists(outputFile))
            logger?.Invoke(LogLevel.Warning, $"Overwriting {outputFile}");

        using var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);

        var files = Directory.GetFiles(inputPath);
        Array.Sort(files, new NaturalComparer());
        var idx = 0;
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
            cbzProgress?.Invoke((double)++idx / files.Length * 100.0);
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
    /// <param name="logger">The logger function</param>
    private static void DownloadCover(Uri url, string filePath, Action<LogLevel, string>? logger = null)
    {
        if (File.Exists(filePath))
            logger?.Invoke(LogLevel.Warning, $"Overwriting {filePath}");
        var imageBytes = HttpClient.GetByteArrayAsync(url).Result;

        using var ms = new MemoryStream(imageBytes);
        using var img = Image.Load(ms);
        img.Save(filePath, new JpegEncoder()
        {
            Quality = 90
        });
    }
}