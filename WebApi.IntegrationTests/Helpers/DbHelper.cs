using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace WebApi.IntegrationTests.Helpers
{
    public static class DbHelper
    {
        public static HttpClient CreateClientWithNewDb<TEntryPoint, TDbContext>(WebApplicationFactory<TEntryPoint> factory, Action<TDbContext> seedDb = null,
            Action<IServiceCollection> configureServices = null)
            where TEntryPoint : class
            where TDbContext : DbContext
        {
            return CreateFactoryWithNewDb(factory, seedDb, configureServices).CreateClient();
        }

        public static WebApplicationFactory<TEntryPoint> CreateFactoryWithNewDb<TEntryPoint, TDbContext>(WebApplicationFactory<TEntryPoint> factory,
            Action<TDbContext> seedDb = null, Action<IServiceCollection> configureServices = null, Dictionary<string, string> configuration = null)
            where TEntryPoint : class
            where TDbContext : DbContext
        {
            return factory.WithWebHostBuilder(builder =>
                builder
                    .ConfigureServices(services =>
                    {
                        var serviceProvider = new ServiceCollection()
                            .AddEntityFrameworkInMemoryDatabase()
                            .BuildServiceProvider();

                        var dbName = "InMemoryAppDb" + Guid.NewGuid();
                        Debug.WriteLine($"Create in memory database '{dbName}'");

                        services.AddDbContext<TDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(dbName);
                            options.UseInternalServiceProvider(serviceProvider);
                        });

                        configureServices?.Invoke(services);

                        var sp = services.BuildServiceProvider();

                        using var scope = sp.CreateScope();
                        var scopedServices = scope.ServiceProvider;
                        var appDb = scopedServices.GetRequiredService<TDbContext>();

                        var logger = scopedServices.GetRequiredService<ILogger<WebApplicationFactory<TEntryPoint>>>();

                        appDb.Database.EnsureCreated();

                        try
                        {
                            seedDb?.Invoke(appDb);
                            appDb.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An error occurred seeding the database with test data.");
                        }
                    })
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    if (configuration != null)
                    {
                        configBuilder.AddInMemoryCollection(configuration);
                    }
                }));
        }

        public static void CheckDb<TEntryPoint, TDbContext>(WebApplicationFactory<TEntryPoint> factory, Action<TDbContext> checkDb) where TEntryPoint : class
        {
            using (var scope = factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<TDbContext>();

                checkDb(appDb);
            }
        }
    }
}
