using MediatR;

namespace PayrollManagementSystem.Application.Features.SystemManagement.Queries.ExportSystemLogs
{
    public class ExportSystemLogsQuery : IRequest<byte[]>
    {
        public string Format { get; set; } = "Excel"; // "Excel" or "PDF"
        public string? Level { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    }
}
