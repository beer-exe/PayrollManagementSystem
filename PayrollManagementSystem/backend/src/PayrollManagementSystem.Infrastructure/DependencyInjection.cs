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
            services.AddScoped<Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor, PayrollManagementSystem.Infrastructure.Persistence.Interceptors.AuditableEntitySaveChangesInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) => {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                options.AddInterceptors(sp.GetServices<Microsoft.EntityFrameworkCore.Diagnostics.ISaveChangesInterceptor>());
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddTransient<IExcelService, ExcelService>();
            services.AddTransient<ITimekeepingCalculatorService, TimekeepingCalculatorService>();

            services.AddHostedService<PayrollManagementSystem.Infrastructure.BackgroundJobs.UpdateExpiredDecisionsJob>();
            services.AddHostedService<PayrollManagementSystem.Infrastructure.BackgroundJobs.CacheWarmingService>();

            var cacheSettings = new PayrollManagementSystem.Application.Common.Models.CacheSettings
            {
                Provider = configuration["CacheSettings:Provider"] ?? "Memory",
                DefaultExpirationInMinutes = int.TryParse(configuration["CacheSettings:DefaultExpirationInMinutes"], out var defaultExp) ? defaultExp : 60,
                RedisConnectionString = configuration["CacheSettings:RedisConnectionString"]
            };

            services.Configure<PayrollManagementSystem.Application.Common.Models.CacheSettings>(opts =>
            {
                opts.Provider = cacheSettings.Provider;
                opts.DefaultExpirationInMinutes = cacheSettings.DefaultExpirationInMinutes;
                opts.RedisConnectionString = cacheSettings.RedisConnectionString;
            });

            if (cacheSettings.Provider == "Redis")
            {
                services.AddMemoryCache();
                services.AddSingleton<MemoryCacheService>();
                services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp =>
                {
                    var options = StackExchange.Redis.ConfigurationOptions.Parse(cacheSettings.RedisConnectionString);
                    options.AllowAdmin = true;
                    return StackExchange.Redis.ConnectionMultiplexer.Connect(options);
                });
                services.AddSingleton<RedisCacheService>();
                services.AddSingleton<ICacheService, ResilientRedisCacheService>();
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
