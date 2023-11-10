using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Claims;
using WebApi.IntegrationTests.Enums;

namespace WebApi.IntegrationTests.Authentication
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimsTransformation(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;
            var newClaimsIdentity = identity.Clone();

            foreach (var authHeaderItem in _httpContextAccessor.HttpContext.Request.Headers.Authorization)
            {
                if (!TryParseToken(authHeaderItem, out var token))
                {
                    continue;
                }

                foreach (var role in GetAppRoles(token))
                {
                    newClaimsIdentity.AddClaim(new Claim(identity.RoleClaimType, role.ToString()));
                }
            }

            return Task.FromResult(new ClaimsPrincipal(newClaimsIdentity));
        }

        private static bool TryParseToken(string authorization, out string token)
        {
            token = string.Empty;

            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith(Constants.Common.TestAuthenticationScheme))
            {
                return false;
            }

            token = authorization.Replace(Constants.Common.TestAuthenticationScheme, string.Empty).Trim();
            return !string.IsNullOrEmpty(token);
        }

        private static IEnumerable<AppRoles> GetAppRoles(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                yield break;
            }

            foreach (var valueItem in token.Split(','))
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
