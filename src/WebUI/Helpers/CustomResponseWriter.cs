using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebUI.Extensions;

public static class CustomResponseWriter
{
    public static Task WriteLive(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json";

        var responseData = new MainHealthResult
        {
            Status = result.Status.ToString(),
            TotalDuration = result.TotalDuration.TotalSeconds.ToString("00:00:00")
        };

        return context.Response.WriteAsJsonAsync(responseData);
    }

    public static Task WriteDependency(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json";

        var responseData = new MainHealthResult
        {
            Status = result.Status.ToString(),
            TotalDuration = result.TotalDuration.TotalSeconds.ToString("00:00:00"),
            Entries = new()
        };

        foreach (var item in result.Entries)
        {
            var subRes = new SubHealthResult
            {
                Tags = item.Value.Tags,
                Status = item.Value.Status.ToString(),
                Description = item.Value.Description,
                Duration = item.Value.Duration.TotalSeconds.ToString("00:00:00"),
                Exception = item.Value.Exception?.Message,
                Data = item.Value.Data?.Count > 0 ? (Dictionary<string, object>)item.Value.Data : null
            };

            responseData.Entries.Add(item.Key, subRes);
        }

        return context.Response.WriteAsJsonAsync(responseData);
    }
}