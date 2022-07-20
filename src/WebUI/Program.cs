using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebUI;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddRazorPages()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.WriteIndented = true);

string connectionString = config.GetConnectionString("SQLSERVER");
var apiUri = new Uri($"{config["API_URL"]}/hc");

builder.Services.AddHealthChecks()
                 .AddSqlServer(connectionString)
                 .AddUrlGroup(apiUri,
                              name: "My API",
                              failureStatus: HealthStatus.Degraded,
                              timeout: TimeSpan.FromSeconds(3));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    ResponseWriter = WriteHealthCheckResponse.Write
});

//app.MapHealthChecks("/health")
//    .RequireAuthorization()
//    .RequireHost("www.test.com:500")
//    .RequireCors("CORS_POLICY");


app.Run();