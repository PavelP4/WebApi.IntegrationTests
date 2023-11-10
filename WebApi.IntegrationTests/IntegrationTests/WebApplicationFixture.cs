using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApi.IntegrationTests.Authentication;

namespace WebApi.IntegrationTests.IntegrationTests
{
    public class WebApplicationFixture : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, TestSchemeAuthenticationHandler>(Constants.Common.TestAuthenticationScheme, null);
                services.AddSingleton<ITokenService, TokenService>();
            });
        }
    }
}
