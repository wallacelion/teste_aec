using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using api.src.shared.infrastructure.data;

namespace api.src.shared.infrastructure.ioc
{
    public static class SharedModuleConfig
    {
        public static IServiceCollection AddSharedModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers(options =>
            {
                options.Filters.Add<api.src.shared.infrastructure.filters.ApiResponseResultFilter>();
            });

            return services;
        }
    }
}
