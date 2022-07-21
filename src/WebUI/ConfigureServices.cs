using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
                         .AddFileWriter("File Writer Check",
                                        failureStatus: HealthStatus.Degraded,
                                        tags: new[] { "ready" });


        services.AddHealthChecksUI(settings =>
        {
            settings.AddHealthCheckEndpoint("WebUI App Live", "/health/live");
            settings.AddHealthCheckEndpoint("WebUI App Dependency", "/health/ready");
        })
        .AddInMemoryStorage();

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

        app.MapHealthChecks("/hc-ui", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            Predicate = _ => true
        });

        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/hc-ui";
            options.ApiPath = "/health-ui-api";
            options.UseRelativeApiPath = false;
        });
    }
}
