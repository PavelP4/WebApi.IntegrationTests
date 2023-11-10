using System.Text;

namespace WebApi.IntegrationTests.Authentication
{
    public class TokenService : ITokenService
    {
        public string Create(string permission)
        {
            if (string.IsNullOrEmpty(permission))
            {
                return string.Empty;
            }

            return Base64Encode(permission);
        }

        public string Parse(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            return Base64Decode(token);
        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string Base64Decode(string base64Encoded)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64Encoded));
        }
    }
}
