using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Features.Departments.Queries.GetAllDepartments;
using PayrollManagementSystem.Application.Features.JobGrades.Queries.GetJobGrades;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetBacThueList;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Queries.GetMucQuyDois;

namespace PayrollManagementSystem.Infrastructure.BackgroundJobs
{
    public class CacheWarmingService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CacheWarmingService> _logger;

        public CacheWarmingService(IServiceProvider serviceProvider, ILogger<CacheWarmingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CacheWarmingService: warming up static caches...");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var warmupTasks = new List<Task>
                {
                    WarmAsync(sender, new GetAllDepartmentsQuery(), "Departments", cancellationToken),
                    WarmAsync(sender, new GetJobGradesQuery(), "JobGrades", cancellationToken),
                    WarmAsync(sender, new GetBacThueListQuery(), "BacThue", cancellationToken),
                    WarmAsync(sender, new GetCauHinhGiamTruQuery(), "CauHinhGiamTru", cancellationToken),
                    WarmAsync(sender, new GetKyDanhGiasQuery(), "KyDanhGia", cancellationToken),
                    WarmAsync(sender, new GetMucQuyDoisQuery(), "MucQuyDoi", cancellationToken),
                };

                await Task.WhenAll(warmupTasks);
                _logger.LogInformation("CacheWarmingService: all static caches warmed up successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CacheWarmingService: error during cache warm-up. Application continues without pre-warmed cache.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task WarmAsync<TResponse>(ISender sender, IRequest<TResponse> request, string name, CancellationToken cancellationToken)
        {
            try
            {
                await sender.Send(request, cancellationToken);
                _logger.LogInformation("CacheWarmingService: warmed [{Name}].", name);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "CacheWarmingService: failed to warm [{Name}].", name);
            }
        }
    }
}
