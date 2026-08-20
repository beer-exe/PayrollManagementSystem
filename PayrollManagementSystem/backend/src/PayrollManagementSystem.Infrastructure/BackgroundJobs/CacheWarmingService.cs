using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Features.CompetencyP2.KyDanhGia.Queries.GetKyDanhGias;
using PayrollManagementSystem.Application.Features.CompetencyP2.MucQuyDoi.Queries.GetMucQuyDois;
using PayrollManagementSystem.Application.Features.Departments.Queries.GetAllDepartments;
using PayrollManagementSystem.Application.Features.JobGrades.Queries.GetJobGrades;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetBacThueList;
using PayrollManagementSystem.Application.Features.ThueTncn.Queries.GetCauHinhGiamTru;

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
                var warmupTasks = new List<Task>
                {
                    WarmAsync(new GetAllDepartmentsQuery(), "Departments", cancellationToken),
                    WarmAsync(new GetJobGradesQuery(), "JobGrades", cancellationToken),
                    WarmAsync(new GetBacThueListQuery(), "BacThue", cancellationToken),
                    WarmAsync(new GetCauHinhGiamTruQuery(), "CauHinhGiamTru", cancellationToken),
                    WarmAsync(new GetKyDanhGiasQuery(), "KyDanhGia", cancellationToken),
                    WarmAsync(new GetMucQuyDoisQuery(), "MucQuyDoi", cancellationToken),
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

        private async Task WarmAsync<TResponse>(IRequest<TResponse> request, string name, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
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
