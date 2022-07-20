var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHealthChecks()
                 .AddSqlServer(builder.Configuration.GetConnectionString("SQLSERVER"));

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
