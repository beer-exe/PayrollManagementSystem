using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PayrollManagementSystem.Infrastructure.Hubs
{
    [Authorize(Roles = "Admin")]
    public class LogMonitorHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminLogMonitor");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminLogMonitor");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
