using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebUI;

public static class WriteHealthCheckResponse
{
    public static Task Write(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json";

        var responseData = new WriteResponseData
        {
            Status = result.Status.ToString(),
            TotalDuration = result.TotalDuration.TotalSeconds.ToString("0:0.00")
        };

        foreach (var item in result.Entries)
        {
            responseData.DependencyHealthChecks.Add(new HealthResult
            {
                Name = item.Key,
                Status = item.Value.Status.ToString(),
                Duration = item.Value.Duration.TotalSeconds.ToString("0:0.00")
            });
        }

        string res = JsonSerializer.Serialize(responseData,
                                              options: new JsonSerializerOptions
                                              {
                                                  WriteIndented = true
                                              });

        return context.Response.WriteAsync(res);
    }
}

internal record WriteResponseData
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("totalDuration")]
    public string TotalDuration { get; set; }

    [JsonPropertyName("dependencyHealthChecks")]
    public List<HealthResult> DependencyHealthChecks { get; set; } = new();
}

internal class HealthResult
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("duration")]
    public string Duration { get; set; }
}
