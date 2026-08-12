using PayrollManagementSystem.Application.Features.Employees.DTOs;

namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface IExcelService
    {
        byte[] ExportEmployeesToExcel(IEnumerable<EmployeeDto> employees);
        byte[] ExportSystemLogsToExcel(IEnumerable<PayrollManagementSystem.Application.Features.SystemManagement.DTOs.SystemLogDto> logs);
    }
}
