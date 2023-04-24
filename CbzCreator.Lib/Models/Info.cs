using Newtonsoft.Json;

namespace CbzCreator.Lib.Models;

public class Info
{
    public enum Statuses
    {
        Unknown,
        Ongoing,
        Completed,
        Licensed,
        PublishingFinished,
        Cancelled,
        OnHiatus
    }

    [JsonProperty("title")]
    public string? Title { get; set; }
    [JsonProperty("author")]
    public string? Author { get; set; }
    [JsonProperty("artist")]
    public string? Artist { get; set; }
    [JsonProperty("description")]
    public string? Description { get; set; }
    [JsonProperty("genre")]
    public List<string>? Genre { get; set; }
    [JsonProperty("status")]
    public Statuses Status { get; set; }

    [JsonIgnore]
    public Uri? CoverUrl { get; set; }
}