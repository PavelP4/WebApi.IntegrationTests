using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using WebApi.IntegrationTests.Enums;

namespace WebApi.IntegrationTests.Attributes
{
    public class AppAuthorize : AuthorizeAttribute
    {
        public AppAuthorize(params AppRoles[] allowedRoles)
        {
            AuthenticationSchemes = IISDefaults.AuthenticationScheme;
            if (allowedRoles != null && allowedRoles.Any())
            {
                Roles = string.Join(',', allowedRoles);
            }
        }
    }
}
