using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollManagementSystem.Application.Behaviours;
using System.Reflection;

namespace PayrollManagementSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
            });

            return services;
        }
    }
}
