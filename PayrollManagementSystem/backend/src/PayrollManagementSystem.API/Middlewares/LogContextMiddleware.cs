using Serilog.Context;
using System.Security.Claims;

namespace PayrollManagementSystem.API.Middlewares
{
    public class LogContextMiddleware
    {
        private readonly RequestDelegate _next;

        public LogContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var traceId = context.TraceIdentifier;

            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("CorrelationId", traceId))
            {
                await _next(context);
            }
        }
    }
}
