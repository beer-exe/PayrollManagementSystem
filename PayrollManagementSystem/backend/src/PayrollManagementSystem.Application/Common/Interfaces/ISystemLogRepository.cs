using PayrollManagementSystem.Application.Features.SystemManagement.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface ISystemLogRepository
    {
        Task<PagedResponse<List<SystemLogDto>>> GetLogsAsync(
            string? level,
            DateTime? fromDate,
            DateTime? toDate,
            string? keyword,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
