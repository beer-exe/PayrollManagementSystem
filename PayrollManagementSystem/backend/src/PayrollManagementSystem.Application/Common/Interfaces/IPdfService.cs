using PayrollManagementSystem.Application.Features.SystemManagement.DTOs;

namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface IPdfService
    {
        byte[] ExportSystemLogsToPdf(IEnumerable<SystemLogDto> logs);
    }
}
