using Microsoft.Extensions.Diagnostics.HealthChecks;
using LatestVersion_ProductionProblem.EF_PeristanceLayer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string connectionString = builder.Configuration.GetConnectionString("SchoolContext");
builder.Services.AddDbContext<SchoolContext>(options =>
  options.UseSqlServer(connectionString));

//https://blog.zhaytam.com/2020/04/30/health-checks-aspnetcore/

builder.Services.AddHealthChecks()
      .AddCheck("AlwaysHealthy", () => HealthCheckResult.Healthy(), tags: new[] { "Tag1" });
builder.Services.AddHealthChecks()
  .AddSqlServer(connectionString);
builder.Services
  .AddHealthChecksUI(s =>
  {
      s.AddHealthCheckEndpoint("endpoint1", "https://localhost:7070/health");
  }).AddSqlServerStorage(connectionString);

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
