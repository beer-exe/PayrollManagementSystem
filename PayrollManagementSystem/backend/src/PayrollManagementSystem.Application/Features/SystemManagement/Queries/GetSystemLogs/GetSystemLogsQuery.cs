using MediatR;
using PayrollManagementSystem.Application.Features.SystemManagement.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SystemManagement.Queries.GetSystemLogs
{
    public class GetSystemLogsQuery : IRequest<PagedResponse<List<SystemLogDto>>>
    {
        public string? Level { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
