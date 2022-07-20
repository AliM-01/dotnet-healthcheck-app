using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddRazorPages();

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
app.MapHealthChecks("/health");
    //.RequireAuthorization()
    //.RequireHost("www.test.com:500")
    //.RequireCors("CORS_POLICY");

app.Run();
