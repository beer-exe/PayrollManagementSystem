using System.Threading.Channels;

namespace PayrollManagementSystem.Infrastructure.Logging
{
    public record LogEventEntry(DateTime RaiseDate, string Level, string Message, string? Exception);

    public class LogEventChannel
    {
        private readonly Channel<LogEventEntry> _channel = Channel.CreateBounded<LogEventEntry>(
            new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false,
            });

        public ChannelWriter<LogEventEntry> Writer => _channel.Writer;
        public ChannelReader<LogEventEntry> Reader => _channel.Reader;
    }
}
