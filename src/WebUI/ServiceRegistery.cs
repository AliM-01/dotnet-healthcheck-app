using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebUI.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceRegistery
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
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
}

public static class MiddlewareRegistery
{
    public static void UseHealthChecksExtension(this WebApplication app)
    {
        var statusCodes = new Dictionary<HealthStatus, int>
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        };

        // Live Check
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            ResultStatusCodes = statusCodes,
            ResponseWriter = CustomResponseWriter.WriteLive,
            Predicate = (check) => !check.Tags.Contains("ready")
        });

        // Dependency Check
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            ResultStatusCodes = statusCodes,
            ResponseWriter = CustomResponseWriter.WriteDependency,
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
            options.ApiPath = "/hc-ui-api";
            options.UseRelativeApiPath = false;
        });
    }
}
