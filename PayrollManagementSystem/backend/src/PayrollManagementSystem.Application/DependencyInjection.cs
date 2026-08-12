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
                // Thứ tự thực thi (FIFO - đăng ký trước = outermost = chạy trước):
                // LoggingBehavior → PerformanceBehavior → ValidationBehaviour → CacheInvalidationBehavior → CachingBehavior → TransactionBehavior → Handler
                // Nhờ đó CacheInvalidation chỉ xóa cache SAU khi Transaction đã commit thành công.
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
                options.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            });

            return services;
        }
    }
}
