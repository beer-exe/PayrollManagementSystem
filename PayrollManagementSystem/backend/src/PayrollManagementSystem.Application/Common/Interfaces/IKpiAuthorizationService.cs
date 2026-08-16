namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface IKpiAuthorizationService
    {
        Task<List<string>> GetSubordinateCccdsAsync(Guid managerTaiKhoanId, CancellationToken cancellationToken);
        Task<bool> CanManageAsync(Guid managerTaiKhoanId, string subordinateCccd, CancellationToken cancellationToken);
    }
}
