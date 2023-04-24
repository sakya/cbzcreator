using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CbzCreatorGui.Models;

public class CoverImage
{
    [JsonProperty("extraLarge")]
    public string? ExtraLarge { get; set; }
}

public class Data
{
    [JsonProperty("Page")]
    public Page? Page { get; set; }
}

public class Edge
{
    [JsonProperty("role")]
    public string? Role { get; set; }

    [JsonProperty("node")]
    public Node? Node { get; set; }
}

public class Medium
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("title")]
    public Title? Title { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("genres")]
    public List<string>? Genres { get; set; }

    [JsonProperty("format")]
    public string? Format { get; set; }

    [JsonProperty("coverImage")]
    public CoverImage? CoverImage { get; set; }

    [JsonProperty("staff")]
    public Staff? Staff { get; set; }
}

public class Name
{
    [JsonProperty("full")]
    public string? Full { get; set; }
}

public class Node
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("name")]
    public Name? Name { get; set; }
}

public class Page
{
    [JsonProperty("media")]
    public List<Medium>? Media { get; set; }
}

public class SearchResult
{
    [JsonProperty("data")]
    public Data? Data { get; set; }
}

public class Staff
{
    [JsonProperty("edges")]
    public List<Edge>? Edges { get; set; }

    public Edge? Author
    {
        get
        {
            var e = Edges?.Where(s => s.Role == "Story & Art")
                .FirstOrDefault();
            if (e != null)
                return e;
            return Edges?.Where(s => s.Role == "Story")
                .FirstOrDefault();
        }
    }

    public Edge? Artist
    {
        get
        {
            var e = Edges?.Where(s => s.Role == "Story & Art")
                .FirstOrDefault();
            if (e != null)
                return e;
            return Edges?.Where(s => s.Role == "Art")
                .FirstOrDefault();
        }
    }

    public override string ToString()
    {
        var e = Edges?.Where(s => s.Role == "Story & Art")
            .FirstOrDefault();
        if (e != null)
            return e.Node?.Name?.Full ?? string.Empty;

        e = Edges?.Where(s => s.Role == "Story")
            .FirstOrDefault();
        if (e != null) {
            var res = e.Node?.Name?.Full;
            e = Edges?.Where(s => s.Role == "Art")
                .FirstOrDefault();
            if (e != null)
                res = $"{res}, {e.Node?.Name?.Full}";
            return res ?? string.Empty;
        }
        return string.Empty;
    }
}

public class Title
{
    [JsonProperty("romaji")]
    public string? Romaji { get; set; }

    [JsonProperty("english")]
    public string? English { get; set; }
}

