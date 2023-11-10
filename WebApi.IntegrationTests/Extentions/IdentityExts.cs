using System.Security.Principal;

namespace WebApi.IntegrationTests.Extentions
{
    public static class IdentityExts
    {
        public static string GetLogin(this IIdentity identity)
        {
            return string.IsNullOrEmpty(identity.Name) ? null : identity
                .Name
                .Split('\\')
                .LastOrDefault();
        }
    }
}
