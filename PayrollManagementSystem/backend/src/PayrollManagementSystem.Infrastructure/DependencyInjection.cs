using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Infrastructure.Persistence;
using PayrollManagementSystem.Infrastructure.Services;

namespace PayrollManagementSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IExcelService, ExcelService>();

            var cacheSettings = new PayrollManagementSystem.Application.Common.Models.CacheSettings
            {
                Provider = configuration["CacheSettings:Provider"] ?? "Memory",
                DefaultExpirationInMinutes = int.TryParse(configuration["CacheSettings:DefaultExpirationInMinutes"], out var defaultExp) ? defaultExp : 60,
                RedisConnectionString = configuration["CacheSettings:RedisConnectionString"]
            };

            if (cacheSettings.Provider == "Redis")
            {
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }

            return services;
        }
    }
}
