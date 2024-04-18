namespace WappuApi.Authentication;

public static class AuthenticationModule
{
    private const string ApiKeyAuthenticationScheme = "ApiKey";

    public static IServiceCollection AddApiKeyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ApiKeyAuthenticationOptions>(ApiKeyAuthenticationScheme)
            .Bind(configuration.GetSection("ApiKey"))
            .ValidateDataAnnotations();

        services.AddAuthentication(ApiKeyAuthenticationScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationScheme, null);

        services.AddAuthorizationBuilder()
            .AddPolicy("ApiKeyPolicy", policy =>
            {
                policy.AddAuthenticationSchemes(ApiKeyAuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        
        return services;
    }
}
