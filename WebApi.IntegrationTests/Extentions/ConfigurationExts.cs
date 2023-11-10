namespace WebApi.IntegrationTests.Extentions
{
    public static class ConfigurationExts
    {
        public static string GetDatabaseConnection(this IConfiguration configuration)
        {
            return configuration.GetConnectionString(Constants.Db.ConnectionName);
        }

        public static string GetDatabaseProvider(this IConfiguration configuration)
        {
            return configuration.GetValue<string>(Constants.Db.ProviderName);
        }
    }
}
