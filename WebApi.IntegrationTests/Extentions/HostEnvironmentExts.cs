namespace WebApi.IntegrationTests.Extentions
{
    public static class HostEnvironmentExts
    {
        public static bool IsTesting(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment(Constants.Common.TestEnvironmentName);
        }
    }
}
