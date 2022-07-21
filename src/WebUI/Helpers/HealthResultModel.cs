using System.Text.Json.Serialization;

namespace WebUI.Extensions;

internal record MainHealthResult
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("totalDuration")]
    public string TotalDuration { get; set; }

    [JsonPropertyName("entries")]
    public Dictionary<string, SubHealthResult> Entries { get; set; }
}

internal record SubHealthResult
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("duration")]
    public string Duration { get; set; }

    [JsonPropertyName("tags")]
    public IEnumerable<string> Tags { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("exception")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
    public string? Exception { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Data { get; set; }
}