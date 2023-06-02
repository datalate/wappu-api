using Microsoft.EntityFrameworkCore;

namespace WappuApi.Core;

public static class CoreModule
{
    public static IServiceCollection AddCoreModule(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetSection("Mock:Database").Get<bool>())
        {
            var dbName = Guid.NewGuid().ToString();

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseInMemoryDatabase(databaseName: dbName);
            });
        }
        else
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(configuration.GetConnectionString("Default"));
            });
        }
        
        return services;
    }
}
