using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebUI.Checks;
using WebUI.Extensions;

namespace WebUI;

public static class ConfigureServices
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorPages()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault;
                    opt.JsonSerializerOptions.WriteIndented = true;
                });

        string connectionString = config.GetConnectionString("SQLSERVER");
        var apiUri = new Uri($"{config["API_URL"]}/hc");

        services.AddHealthChecks()
                         .AddSqlServer(connectionString, tags: new[] { "ready" })
                         .AddUrlGroup(apiUri,
                                      name: "My API",
                                      failureStatus: HealthStatus.Degraded,
                                      timeout: TimeSpan.FromSeconds(3),
                                      tags: new[] { "ready" })
                         .AddCheck<FileWriterCheck>("File Writer Check",
                                                    failureStatus: HealthStatus.Degraded,
                                                    tags: new[] { "ready" });

        return services;
    }

    public static void UseHealthChecksExtension(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            ResultStatusCodes = HealthCheckResultStatusCodes.Codes(),
            ResponseWriter = WriteHealthCheckResponse.WriteLive,
            Predicate = (check) => !check.Tags.Contains("ready")
        });

        // Dependency Check
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            ResultStatusCodes = HealthCheckResultStatusCodes.Codes(),
            ResponseWriter = WriteHealthCheckResponse.WriteDependency,
            Predicate = (check) => check.Tags.Contains("ready")
        });
    }
}
