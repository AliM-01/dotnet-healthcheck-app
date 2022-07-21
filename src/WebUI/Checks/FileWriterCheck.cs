using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebUI.Checks;

public class FileWriterCheck : IHealthCheck
{
    private readonly ILogger<FileWriterCheck> _logger;
    private readonly string _filePath;
    private IReadOnlyDictionary<string, object> _healthCheckData;

    public FileWriterCheck(ILoggerFactory loggerFactory, IWebHostEnvironment env)
    {
        _logger = loggerFactory.CreateLogger<FileWriterCheck>();
        _filePath = $"{env.WebRootPath}\\data\\secret\\";
        _healthCheckData = new Dictionary<string, object>
        {
            { "FilePath", _filePath }
        };
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            string tempFile = $"{_filePath}\\temp.txt";
            var fs = File.Create(tempFile);

            fs.Close();

            File.Delete(tempFile);

            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception ex)
        {
            HealthCheckResult result;

            const string message = "File Path Write Check Failed!";

            switch (context.Registration.FailureStatus)
            {
                case HealthStatus.Degraded:
                    result = HealthCheckResult.Degraded(message, ex, _healthCheckData);
                    break;

                case HealthStatus.Healthy:
                    result = HealthCheckResult.Healthy(message, _healthCheckData);
                    break;

                default:
                    result = HealthCheckResult.Unhealthy(message, ex, _healthCheckData);
                    break;
            }


            _logger.LogError("File Path Write Check Failed. Error : {message}", ex.Message);

            return Task.FromResult(result);
        }
    }
}
