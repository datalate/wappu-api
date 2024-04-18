namespace WappuApi.Authentication;

public static class AuthenticationModule
{
    public static IServiceCollection AddApiKeyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiKeyAuthenticationOptions>("ApiKey", configuration.GetSection("ApiKey"));

        services.AddAuthentication("ApiKey")
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        services.AddAuthorizationBuilder()
            .AddPolicy("ApiKeyPolicy", policy =>
            {
                policy.AddAuthenticationSchemes("ApiKey");
                policy.RequireAuthenticatedUser();
            });
        
        return services;
    }
}
