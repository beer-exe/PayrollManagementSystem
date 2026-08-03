namespace PayrollManagementSystem.Application.Features.SystemManagement.DTOs
{
    public class SystemLogDto
    {
        public long Id { get; set; }
        public DateTime RaiseDate { get; set; }
        public string Level { get; set; } = null!;
        public string? Message { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; }
    }
}
