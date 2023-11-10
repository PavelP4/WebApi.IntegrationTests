using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests;
using WebApi.IntegrationTests.Extentions;
using WebApi.IntegrationTests.Infrastructure;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
        webBuilder.ConfigureAppConfiguration(confBuilder => confBuilder.AddEnvironmentVariables());
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var environment = services.GetRequiredService<IWebHostEnvironment>();

    if (!environment.IsTesting())
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }
}

host.Run();