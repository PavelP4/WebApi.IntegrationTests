using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.IntegrationTests.Authentication;
using WebApi.IntegrationTests.Enums;
using WebApi.IntegrationTests.Extentions;
using WebApi.IntegrationTests.Helpers;

namespace WebApi.IntegrationTests.IntegrationTests
{
    public abstract class BaseControllerTest<TEntryPoint, TDbContext>
        where TEntryPoint : class
        where TDbContext : DbContext
    {
        public const string Sequential = "Sequential";

        protected WebApplicationFactory<TEntryPoint> Factory { get; }
        protected abstract string ControllerName { get; }

        private IConfiguration _configuration;
        protected IConfiguration Configuration => _configuration ??= Factory.Services.GetRequiredService<IConfiguration>();

        ITokenService _tokenService;
        protected ITokenService TokenService => _tokenService ??= Factory.Services.GetRequiredService<ITokenService>();

        protected IServiceProvider Services => Factory.Services;
        

        protected BaseControllerTest(
            WebApplicationFactory<TEntryPoint> factory)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Constants.Common.TestEnvironmentName);
            Environment.SetEnvironmentVariable(Constants.Db.ProviderName, DbProvider.InMemory.ToString());
            Factory = factory;
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        private string CombineUrl(string urlPart)
        {
            if (!string.IsNullOrEmpty(urlPart) && !urlPart.StartsWith('?') && !urlPart.StartsWith('/'))
            {
                urlPart = '/' + urlPart;
            }
            return $"api/{ControllerName}{urlPart}";
        }

        public async Task RunTest<TResponse>(Action<TDbContext> seedDb, string urlPart, string permission, Action<TResponse> checkAction)
        {
            await (await RunTest(seedDb, urlPart, permission)).CheckResultAsync(checkAction);
        }

        public async Task RunTest<TResponse>(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission, Action<TResponse> checkAction)
        {
            await (await RunTest(seedDb, urlPart, permission, configureServices)).CheckResultAsync(checkAction);
        }

        public async Task RunTestBadRequest(Action<TDbContext> seedDb, string urlPart, string permission)
        {
            (await RunTest(seedDb, urlPart, permission)).IsBadRequest();
        }

        public async Task RunTestBadRequest(Action<TDbContext> seedDb, string urlPart, string permission, string checkContent)
        {
            await (await RunTest(seedDb, urlPart, permission)).IsBadRequest(checkContent);
        }

        public async Task RunTestBadRequest(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission, string checkContent)
        {
            await (await RunTest(seedDb, urlPart, permission, configureServices)).IsBadRequest(checkContent);
        }


        public async Task RunTestNotFound(Action<TDbContext> seedDb, string urlPart, string permission)
        {
            (await RunTest(seedDb, urlPart, permission)).IsNotFound();
        }

        public async Task RunTestNotFound(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission)
        {
            (await RunTest(seedDb, urlPart, permission, configureServices)).IsNotFound();
        }

        public async Task RunTestForbidden(Action<TDbContext> seedDb, string urlPart, string permission)
        {
            (await RunTest(seedDb, urlPart, permission)).IsForbidden();
        }

        public async Task RunTestRedirect(Action<TDbContext> seedDb, string urlPart, string permission, string checkContent)
        {
            await (await RunTest(seedDb, urlPart, permission)).IsRedirect(checkContent);
        }

        public async Task RunTestRedirect(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission, string checkContent)
        {
            await (await RunTest(seedDb, urlPart, permission, configureServices)).IsRedirect(checkContent);
        }

        private async Task<HttpResponseMessage> RunTest(Action<TDbContext> seedDb, string urlPart, string permission,
            Action<IServiceCollection> configureServices = null)
        {
            return await Call(seedDb, urlPart, permission, (client, url, token) => client.GetAsync(url, token), configureServices);
        }

        private async Task<HttpResponseMessage> Call(Action<TDbContext> seedDb, string urlPart, string permission,
            Func<HttpClient, string, string, Task<HttpResponseMessage>> func, Action<IServiceCollection> configureServices = null,
            Dictionary<string, string> configuration = null)
        {
            using var factory = CreateFactoryWithNewDb(seedDb, configureServices, configuration);
            using var client = CreateClient(factory);
            var token = TokenService.Create(permission);
            return await func(client, CombineUrl(urlPart), token);
        }

        public async Task RunTestPost<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb)
        {
            await RunTestPost<TRequest, object>(seedDb, urlPart, permission, payload, checkDb);
        }

        public async Task<HttpResponseMessage> RunTestPost<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Stream fileContent, Action<TDbContext> checkDb)
        {
            return await RunTestPost<TRequest, HttpResponseMessage>(seedDb, urlPart, permission, payload, checkDb, fileContent: fileContent);
        }


        public async Task<HttpResponseMessage> RunTestPost<TRequest, TResponse>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb,
            Action<TResponse> checkAction = null, Stream fileContent = null)
        {
            return await RunTestPostPutSuccess(true, seedDb, urlPart, permission, payload, checkDb, checkAction, fileContent: fileContent);
        }

        public async Task RunTestPost<TRequest, TResponse>(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices,
            string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb,
            Action<TResponse> checkAction, Stream fileContent)
        {
            await RunTestPostPutSuccess(true, seedDb, urlPart, permission, payload, checkDb, checkAction, fileContent: fileContent, configureServices: configureServices);
        }

        public async Task RunTestPost<TRequest>(Action<TDbContext> seedDb,
            Action<IServiceCollection> configureServices, string urlPart, string permission, TRequest payload,
            Action<TDbContext> checkDb)
        {
            await RunTestPost<TRequest, object>(seedDb, configureServices, urlPart, permission, payload, checkDb, null);
        }

        public async Task RunTestPost<TRequest, TResponse>(Action<TDbContext> seedDb,
            Action<IServiceCollection> configureServices, string urlPart, string permission, TRequest payload,
            Action<TDbContext> checkDb, Action<TResponse> checkAction)
        {
            await RunTestPostPutSuccess(true, seedDb, urlPart, permission, payload, checkDb, checkAction, configureServices);
        }

        public async Task RunTestPost<TRequest, TResponse>(Action<TDbContext> seedDb,
            Action<IServiceCollection> configureServices, Dictionary<string, string> configuration, string urlPart, string permission, TRequest payload,
            Action<TDbContext> checkDb, Action<TResponse> checkAction)
        {
            await RunTestPostPutSuccess(true, seedDb, urlPart, permission, payload, checkDb, checkAction, configureServices, configuration);
        }

        public async Task RunTestPut<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb)
        {
            await RunTestPut<TRequest, object>(seedDb, urlPart, permission, payload, checkDb);
        }

        public async Task RunTestPut<TRequest>(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb)
        {
            await RunTestPut<TRequest, object>(seedDb, configureServices, urlPart, permission, payload, checkDb);
        }

        public async Task RunTestPut<TRequest, TResponse>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb, Action<TResponse> checkAction = null)
        {
            await RunTestPostPutSuccess(false, seedDb, urlPart, permission, payload, checkDb, checkAction);
        }

        public async Task RunTestPut<TRequest, TResponse>(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb, Action<TResponse> checkAction = null)
        {
            await RunTestPostPutSuccess(false, seedDb, urlPart, permission, payload, checkDb, checkAction, configureServices);
        }

        public async Task RunTestPutBadRequest<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload,
            string checkContentPart = null)
        {
            await (await RunTestPut(seedDb, urlPart, permission, payload)).IsBadRequest(checkContentPart);
        }

        public async Task RunTestPutForbidden<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload,
            string checkContentPart = null)
        {
            await (await RunTestPut(seedDb, urlPart, permission, payload)).IsForbidden(checkContentPart);
        }

        public async Task RunTestPutNotFound<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload)
        {
            (await RunTestPut(seedDb, urlPart, permission, payload)).IsNotFound();
        }

        private async Task<HttpResponseMessage> RunTestPut<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload)
        {
            return await Call(seedDb, urlPart, permission, (client, url, token) => client.PutAsync(url, payload, token));
        }

        public async Task RunTestPostBadRequest<TRequest>(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices,
            string urlPart, string permission, TRequest payload, string checkContentPart = null)
        {
            await (await RunTestPostNotSuccess(seedDb, urlPart, permission, payload, configureServices)).IsBadRequest(checkContentPart);
        }

        public async Task RunTestPostBadRequest<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload)
        {
            (await RunTestPostNotSuccess(seedDb, urlPart, permission, payload)).IsBadRequest();
        }

        public async Task RunTestPostBadRequest<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, string checkContentPart)
        {
            await (await RunTestPostNotSuccess(seedDb, urlPart, permission, payload)).IsBadRequest(checkContentPart);
        }


        public async Task RunTestPostBadRequest<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Stream fileContent, string checkContentPart = null)
        {
            await (await RunTestPostNotSuccess(seedDb, urlPart, permission, payload, null, fileContent)).IsBadRequest(checkContentPart);
        }


        public async Task RunTestPostForbidden<TRequest>(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission, TRequest payload)
        {
            (await RunTestPostNotSuccess(seedDb, urlPart, permission, payload, configureServices)).IsForbidden();
        }

        public async Task RunTestPostForbidden<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload,
            string checkContentPart = null)
        {
            await (await RunTestPostNotSuccess(seedDb, urlPart, permission, payload)).IsForbidden(checkContentPart);
        }

        public async Task RunTestPostNotFound<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload,
            Action<IServiceCollection> configureServices = null)
        {
            (await RunTestPostNotSuccess(seedDb, urlPart, permission, payload, configureServices)).IsNotFound();
        }

        private async Task<HttpResponseMessage> RunTestPostNotSuccess<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload,
            Action<IServiceCollection> configureServices = null, Stream fileContent = null)
        {
            return await Call(seedDb, urlPart, permission, (client, url, token) => client.PostAsync(url, payload, token, fileContent), configureServices);
        }

        public async Task RunTestDelete(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission,
            Action<TDbContext> checkDb)
        {
            await RunTestDelete(seedDb, urlPart, permission, checkDb, configureServices);
        }

        public async Task RunTestDelete<TRequest>(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices, string urlPart, string permission,
            TRequest payload, Action<TDbContext> checkDb)
        {
            await RunTestDelete(seedDb, urlPart, permission, payload, checkDb, configureServices);
        }

        public async Task RunTestDelete(Action<TDbContext> seedDb, string urlPart, string permission, Action<TDbContext> checkDb,
            Action<IServiceCollection> configureServices = null)
        {
            await RunTestDelete<object>(seedDb, urlPart, permission, null, checkDb, configureServices);
        }

        public async Task RunTestDelete<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb,
            Action<IServiceCollection> configureServices = null)
        {
            using var factory = CreateFactoryWithNewDb(seedDb, configureServices);
            using var client = CreateClient(factory);
            var token = TokenService.Create(permission);
            await (await client.DeleteAsync(CombineUrl(urlPart), payload, token)).IsSuccessStatusCode();

            DbHelper.CheckDb(factory, checkDb);
        }

        public async Task RunTestDeleteNotFound(Action<TDbContext> seedDb, string urlPart, string permission, Action<IServiceCollection> configureServices = null)
        {
            (await RunTestDelete(seedDb, urlPart, permission, configureServices)).IsNotFound();
        }

        public async Task RunTestDeleteBadRequest(Action<TDbContext> seedDb, string urlPart, string permission, Action<IServiceCollection> configureServices = null)
        {
            (await RunTestDelete(seedDb, urlPart, permission, configureServices)).IsBadRequest();
        }

        public async Task RunTestDeleteBadRequest<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<IServiceCollection> configureServices = null)
        {
            (await RunTestDelete(seedDb, urlPart, permission, payload, configureServices)).IsBadRequest();
        }

        public async Task RunTestDeleteForbidden(Action<TDbContext> seedDb, string urlPart, string permission, Action<IServiceCollection> configureServices = null,
            string checkContentPart = null)
        {
            await (await RunTestDelete(seedDb, urlPart, permission, configureServices)).IsForbidden(checkContentPart);
        }

        private async Task<HttpResponseMessage> RunTestDelete(Action<TDbContext> seedDb, string urlPart, string permission, Action<IServiceCollection> configureServices = null)
        {
            return await Call(seedDb, urlPart, permission, (client, url, token) => client.DeleteAsync(url, token), configureServices);
        }

        private async Task<HttpResponseMessage> RunTestDelete<TRequest>(Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<IServiceCollection> configureServices = null)
        {
            return await Call(seedDb, urlPart, permission, (client, url, token) => client.DeleteAsync(url, payload, token), configureServices);
        }

        private WebApplicationFactory<TEntryPoint> CreateFactoryWithNewDb(Action<TDbContext> seedDb, Action<IServiceCollection> configureServices,
            Dictionary<string, string> configuration = null)
        {
            return DbHelper.CreateFactoryWithNewDb(Factory, seedDb,
                services =>
                {
                    ConfigureServices(services);
                    configureServices?.Invoke(services);
                },
                configuration);
        }

        private HttpClient CreateClient(WebApplicationFactory<TEntryPoint> factory)
        {
            factory.ClientOptions.AllowAutoRedirect = false;
            var client = factory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(60);
            return client;
        }

        private async Task<HttpResponseMessage> RunTestPostPutSuccess<TRequest, TResponse>(bool post, Action<TDbContext> seedDb, string urlPart, string permission, TRequest payload, Action<TDbContext> checkDb,
            Action<TResponse> checkAction = null, Action<IServiceCollection> configureServices = null, Dictionary<string, string> configuration = null,
            Stream fileContent = null)
        {
            using var factory = CreateFactoryWithNewDb(seedDb, configureServices, configuration);
            using var client = CreateClient(factory);
            var url = CombineUrl(urlPart);
            var token = TokenService.Create(permission);

            var response = post
                ? await client.PostAsync(url, payload, token, fileContent)
                : await client.PutAsync(url, payload, token, fileContent);

            await response.IsSuccessStatusCode();

            if (checkAction != null)
            {
                await response.CheckResultAsync(checkAction);
            }

            if (checkDb != null)
            {
                DbHelper.CheckDb(factory, checkDb);
            }

            return response;
        }
    }
}
