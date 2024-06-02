namespace Kvarovi.Middleware.ApiKeyAuthentication;

using Microsoft.AspNetCore.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ClientKey";
    public const string HeaderName = "x-api-key";
}
