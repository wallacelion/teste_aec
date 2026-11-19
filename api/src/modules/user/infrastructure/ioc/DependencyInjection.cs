using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using api.src.modules.user.application.interfaces;
using api.src.modules.user.application.services;
using api.src.modules.user.domain.interfaces;
using api.src.modules.user.infrastructure.context;
using api.src.modules.user.infrastructure.repositories;

namespace api.src.modules.user.infrastructure.ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
