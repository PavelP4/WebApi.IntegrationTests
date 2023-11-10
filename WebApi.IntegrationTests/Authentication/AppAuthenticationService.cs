using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace WebApi.IntegrationTests.Authentication
{
    public class AppAuthenticationService : AuthenticationService
    {
        public AppAuthenticationService(
          IAuthenticationSchemeProvider schemes,
          IAuthenticationHandlerProvider handlers,
          IClaimsTransformation transform,
          IOptions<AuthenticationOptions> options)
            : base(schemes, handlers, transform, options)
        {
        }

        public override async Task<AuthenticateResult> AuthenticateAsync(
          HttpContext context,
          string scheme)
        {
            if (scheme == null)
            {
                scheme = (await this.Schemes.GetDefaultAuthenticateSchemeAsync())?.Name;
                if (scheme == null)
                    throw new InvalidOperationException($"No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).");
            }
            
            var handlerAsync = await Handlers.GetHandlerAsync(context, scheme);
            if (handlerAsync == null)
            {
                throw new InvalidOperationException(string.Format("No authentication handler is configured to authenticate for the scheme: {0}", (object)scheme));
            }
            
            var result = await handlerAsync.AuthenticateAsync();
            return result;

            //var identity = new ClaimsIdentity();
            //var principal = new ClaimsPrincipal(identity);

            //try
            //{
            //    return AuthenticateResult.Success(new AuthenticationTicket(await Transform.TransformAsync(principal), null, Constants.Common.TestAuthenticationScheme));
            //}
            //catch (Exception ex)
            //{
            //    return AuthenticateResult.Fail(ex);
            //}
        }
    }
}
