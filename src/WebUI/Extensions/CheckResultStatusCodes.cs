using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebUI.Extensions;

public static class HealthCheckResultStatusCodes
{
    public static IDictionary<HealthStatus, int> Codes()
    {
        return new Dictionary<HealthStatus, int>
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        };
    }
}
