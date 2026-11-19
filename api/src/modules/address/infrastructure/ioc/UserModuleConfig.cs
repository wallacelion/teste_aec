using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using api.src.modules.address.application.interfaces;
using api.src.modules.address.application.services;
using api.src.modules.address.domain.interfaces;
using api.src.modules.address.infrastructure.context;
using api.src.modules.address.infrastructure.repositories;
using api.src.modules.address.infrastructure.external.interfaces;
using api.src.modules.address.infrastructure.external.services;
using api.src.modules.address.infrastructure.cache.interfaces;
using api.src.modules.address.infrastructure.cache.services;

namespace api.src.modules.address.infrastructure.ioc
{
    public static class AddressModuleConfig
    {
        public static IServiceCollection AddAddressModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DbContext with the connection string from configuration
            services.AddDbContext<AddressDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register Redis Cache
            var redisConnection = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                    options.InstanceName = "ViaCep_";
                });
            }

            // Register HttpClient for ViaCepService
            services.AddHttpClient<IViaCepService, ViaCepService>();

            // Register Cache Service
            services.AddScoped<IViaCepCacheService, ViaCepCacheService>();

            // Register the AddressRepository and AddressService
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IAddressService, AddressService>();

            return services;
        }
    }
}
