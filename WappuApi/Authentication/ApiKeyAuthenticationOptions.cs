using Microsoft.AspNetCore.Authentication;

namespace WappuApi.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Default { get; set; } = null!;
}