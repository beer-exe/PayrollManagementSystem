using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayrollManagementSystem.Infrastructure.Hubs;
using PayrollManagementSystem.Infrastructure.Logging;

namespace PayrollManagementSystem.Infrastructure.BackgroundJobs
{
    public class LogBroadcastService : BackgroundService
    {
        private readonly LogEventChannel _channel;
        private readonly IHubContext<LogMonitorHub> _hubContext;
        private readonly ILogger<LogBroadcastService> _logger;

        public LogBroadcastService(LogEventChannel channel, IHubContext<LogMonitorHub> hubContext, ILogger<LogBroadcastService> logger)
        {
            _channel = channel;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var entry in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await _hubContext.Clients.Group("AdminLogMonitor")
                        .SendAsync("ReceiveLog", new
                        {
                            entry.RaiseDate,
                            entry.Level,
                            entry.Message,
                            entry.Exception
                        }, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // swallow — không để hub lỗi crash broadcast loop
                }
            }
        }
    }
}
