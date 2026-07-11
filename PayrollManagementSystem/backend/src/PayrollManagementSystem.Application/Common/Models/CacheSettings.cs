namespace PayrollManagementSystem.Application.Common.Models;

public class CacheSettings
{
    public string Provider { get; set; } = "Memory"; // "Memory" or "Redis"
    public int DefaultExpirationInMinutes { get; set; } = 60;
    public string? RedisConnectionString { get; set; }
}
