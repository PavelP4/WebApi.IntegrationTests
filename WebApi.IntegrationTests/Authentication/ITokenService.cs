namespace WebApi.IntegrationTests.Authentication
{
    public interface ITokenService
    {
        string Create(string permission);
        string Parse(string token);
    }
}
