var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/ok", () => Results.Ok("OK !"));

app.MapHealthChecks("hc");

app.Run();