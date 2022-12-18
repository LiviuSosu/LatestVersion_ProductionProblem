using HealthChecks.UI.Client;
using LatestVersion_ProductionProblem.Persistance;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using LatestVersion_ProductionProblem.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string connectionString = builder.Configuration.GetConnectionString("CollegeContext");
builder.Services.AddDbContext<CollegeContext>(options =>
  options.UseSqlServer(connectionString));

builder.Services.AddHealthChecks()
  .AddSqlServer(connectionString);
builder.Services
  .AddHealthChecksUI(s =>
  {
      s.AddHealthCheckEndpoint("endpoint1", "https://localhost:7070/health");
  })
  //.AddInMemoryStorage();
  .AddSqlServerStorage(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CollegeContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


//AspNetCore.HealthChecks.SqlServer
//AspNetCore.HealthChecks.UI.SqlServer.Storage
//Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
//Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions

app.UseRouting().UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecksUI();

    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

app.Run();
