namespace PayrollManagementSystem.Domain.Models
{
    public class SystemLog
    {
        public long Id { get; set; }
        public string? Message { get; set; }
        public string? MessageTemplate { get; set; }
        public string Level { get; set; } = null!;
        public DateTime RaiseDate { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; }
    }
}
