using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Application.Features.Departments.Commands.ExpirePastDecisions;

namespace PayrollManagementSystem.Infrastructure.BackgroundJobs
{
    public class UpdateExpiredDecisionsJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UpdateExpiredDecisionsJob> _logger;

        public UpdateExpiredDecisionsJob(IServiceProvider serviceProvider, ILogger<UpdateExpiredDecisionsJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdateExpiredDecisionsJob is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                        
                        _logger.LogInformation("UpdateExpiredDecisionsJob is running to expire past decisions.");
                        await sender.Send(new ExpirePastDecisionsCommand(), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing UpdateExpiredDecisionsJob.");
                }

                // Wait until the next midnight
                var now = DateTime.Now;
                var nextMidnight = now.Date.AddDays(1);
                var delay = nextMidnight - now;
                
                _logger.LogInformation($"UpdateExpiredDecisionsJob is sleeping for {delay.TotalHours:F2} hours until next run.");

                // For testing/development, this delay could be shortened, 
                // but in production it should wait until 00:00.
                await Task.Delay(delay, stoppingToken);
            }

            _logger.LogInformation("UpdateExpiredDecisionsJob is stopping.");
        }
    }
}
