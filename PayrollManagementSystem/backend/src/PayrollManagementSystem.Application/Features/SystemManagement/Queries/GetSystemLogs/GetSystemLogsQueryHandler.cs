using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SystemManagement.DTOs;
using PayrollManagementSystem.Application.Wrappers;

namespace PayrollManagementSystem.Application.Features.SystemManagement.Queries.GetSystemLogs
{
    public class GetSystemLogsQueryHandler : IRequestHandler<GetSystemLogsQuery, PagedResponse<List<SystemLogDto>>>
    {
        private readonly ISystemLogRepository _repository;

        public GetSystemLogsQueryHandler(ISystemLogRepository repository)
        {
            _repository = repository;
        }

        public Task<PagedResponse<List<SystemLogDto>>> Handle(GetSystemLogsQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetLogsAsync(
                request.Level,
                request.FromDate,
                request.ToDate,
                request.Keyword,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
        }
    }
}
