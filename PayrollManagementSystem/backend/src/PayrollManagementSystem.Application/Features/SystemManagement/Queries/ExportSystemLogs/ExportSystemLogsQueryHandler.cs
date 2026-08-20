using MediatR;
using PayrollManagementSystem.Application.Common.Interfaces;

namespace PayrollManagementSystem.Application.Features.SystemManagement.Queries.ExportSystemLogs
{
    public class ExportSystemLogsQueryHandler : IRequestHandler<ExportSystemLogsQuery, byte[]>
    {
        private readonly ISystemLogRepository _systemLogRepository;
        private readonly IExcelService _excelService;
        private readonly IPdfService _pdfService;

        public ExportSystemLogsQueryHandler(
            ISystemLogRepository systemLogRepository,
            IExcelService excelService,
            IPdfService pdfService)
        {
            _systemLogRepository = systemLogRepository;
            _excelService = excelService;
            _pdfService = pdfService;
        }

        public async Task<byte[]> Handle(ExportSystemLogsQuery request, CancellationToken cancellationToken)
        {
            // Set limit 5000 to prevent OOM
            var logsResponse = await _systemLogRepository.GetLogsAsync(
                request.Level, request.FromDate, request.ToDate, request.Keyword,
                request.SortBy, request.SortDirection,
                1, 5000, cancellationToken);

            var logs = logsResponse.Data ?? new List<DTOs.SystemLogDto>();

            if (request.Format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                return _pdfService.ExportSystemLogsToPdf(logs);
            }

            return _excelService.ExportSystemLogsToExcel(logs);
        }
    }
}
