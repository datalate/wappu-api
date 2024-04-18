using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WappuApi.Authentication;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKeyValues))
        {
            return AuthenticateResult.Fail("Missing API key (Header: X-Api-Key)");
        }

        var providedApiKey = apiKeyValues.FirstOrDefault();
        var expectedApiKey = Options.Default;

        if (string.IsNullOrEmpty(providedApiKey) || providedApiKey != expectedApiKey)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}