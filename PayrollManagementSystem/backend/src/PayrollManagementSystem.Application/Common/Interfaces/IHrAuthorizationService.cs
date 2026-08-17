namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface IHrAuthorizationService
    {
        Task<bool> IsHrManagerAsync(Guid? userId, CancellationToken cancellationToken = default);
    }
}
