using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using WebApi.IntegrationTests.Authentication;
using WebApi.IntegrationTests.Extentions;
using WebApi.IntegrationTests.Infrastructure;
using WebApi.IntegrationTests.Mappings;
using WebApi.IntegrationTests.Repositories;
using WebApi.IntegrationTests.Services;

namespace WebApi.IntegrationTests
{
    public class Startup
    {
        public readonly IConfiguration _configuration;
        public readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment) 
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (!_environment.IsTesting())
            {
                services.AddAuthentication(IISDefaults.AuthenticationScheme);
                services.AddScoped<IAuthenticationService, AppAuthenticationService>();
                services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, TestSchemeAuthenticationHandler>(Constants.Common.TestAuthenticationScheme, null);
                services.AddSingleton<ITokenService, TokenService>();
            }
            

            var mapper = new MapperConfiguration(x =>
            {
                x.AddProfile(new OrderMapping());
                x.AddProfile(new StoreMapping());
            }).CreateMapper();

            services.AddSingleton(mapper);

            services.AddAuthorization();
            services.AddControllers();

            services.AddApplicationDbContext<AppDbContext>(
                _configuration.GetDatabaseProvider(), 
                _configuration.GetDatabaseConnection(), 
                options => options.UseLazyLoadingProxies());

            services.AddScoped(typeof(BaseRepository<>));

            services.AddTransient<OrderService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
