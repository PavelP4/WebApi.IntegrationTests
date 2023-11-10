using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests.Enums;

namespace WebApi.IntegrationTests.Extentions
{
    public static class ServiceCollectionExts
    {
        public static IServiceCollection AddApplicationDbContext<TDbContext>(
            this IServiceCollection serviceCollection,
            string databaseProvider,
            string databaseConnection,
            Action<DbContextOptionsBuilder> optionsAction = null) where TDbContext : DbContext
        {
            var dbProvider = ParseDbProvider(databaseProvider);
            serviceCollection.AddDbContext<TDbContext>(x =>
            {
                OptionsAction(x, dbProvider, databaseConnection);
                optionsAction?.Invoke(x);
             });
            
            return serviceCollection;
        }

        private static DbProvider ParseDbProvider(string databaseProvider)
        {
            if (string.IsNullOrWhiteSpace(databaseProvider))
            {
                return DbProvider.MsSql;
            }

            if (!Enum.TryParse(databaseProvider, true, out DbProvider dbProvider))
            {
                throw new NotSupportedException($"Database provider '{databaseProvider}' not supported. Supported database providers: 'MSSQL', 'InMemory'.");
            }

            return dbProvider;
        }

        private static void OptionsAction(DbContextOptionsBuilder options, DbProvider dbProvider, string databaseConnection)
        {
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
            switch (dbProvider)
            {
                case DbProvider.MsSql:
                    options.UseSqlServer(databaseConnection);
                    break;
                case DbProvider.InMemory:
                    options.UseInMemoryDatabase("InMemory");
                    break;
            }
        }
    }
}
