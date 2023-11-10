using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WebApi.IntegrationTests.Enums;

namespace WebApi.IntegrationTests.Authentication
{
    public class TestSchemeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";

        private readonly ITokenService _tokenService;

        public TestSchemeAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITokenService tokenService)
            : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                return AuthenticateResult.NoResult();
            }

            if (!Constants.Common.TestAuthenticationScheme.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            var token = headerValue.Parameter;
            if (!string.IsNullOrEmpty(token))
            {
                var permission = _tokenService.Parse(token);
                var identity = new ClaimsIdentity(Scheme.Name);
                foreach (var role in GetAppRoles(permission))
                {
                    identity.AddClaim(new Claim(identity.RoleClaimType, role.ToString()));
                }
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Constants.Common.TestAuthenticationScheme);
                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("No token");
        }

        private static IEnumerable<AppRoles> GetAppRoles(string permission)
        {
            if (string.IsNullOrEmpty(permission))
            {
                yield break;
            }

            foreach (var valueItem in permission.Split(','))
            {
                if (!string.IsNullOrEmpty(valueItem) && Enum.TryParse(valueItem, true, out AppRoles value))
                {
                    yield return value;
                }
            }

            yield break;
        }
    }
}
