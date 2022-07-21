using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebUI.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

public static class FileWriterCheckServiceCollectionExtension
{
    public static IHealthChecksBuilder AddFileWriter(this IHealthChecksBuilder builder, string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null)
    {
        return builder.Add(new HealthCheckRegistration(name ?? "filewriter",
                                                       (IServiceProvider sp) => {
                                                           return new FileWriterCheck(sp.GetRequiredService<ILoggerFactory>(), sp.GetRequiredService<IWebHostEnvironment>());
                                                       },
                                                       failureStatus,
                                                       tags));
    }
}
