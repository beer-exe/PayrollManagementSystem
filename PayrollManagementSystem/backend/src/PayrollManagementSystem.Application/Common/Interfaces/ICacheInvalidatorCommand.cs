namespace PayrollManagementSystem.Application.Common.Interfaces;

public interface ICacheInvalidatorCommand
{
    string CacheKeyPrefix { get; }
}
