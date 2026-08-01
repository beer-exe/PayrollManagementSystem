using Serilog.Core;
using Serilog.Events;

namespace PayrollManagementSystem.Infrastructure.Logging
{
    public class SignalRLogSink : ILogEventSink
    {
        private readonly LogEventChannel _channel;

        public SignalRLogSink(LogEventChannel channel)
        {
            _channel = channel;
        }

        public void Emit(LogEvent logEvent)
        {
            var entry = new LogEventEntry(
                logEvent.Timestamp.UtcDateTime,
                logEvent.Level.ToString(),
                logEvent.RenderMessage(),
                logEvent.Exception?.ToString()
            );

            _channel.Writer.TryWrite(entry);
        }
    }
}
