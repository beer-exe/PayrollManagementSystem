namespace PayrollManagementSystem.Application.Features.Kpi.DTOs
{
    public class KyKpiDto
    {
        public Guid IdKyKpi { get; set; }
        public string TenKyKpi { get; set; } = null!;
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string TrangThai { get; set; } = null!;
        public int TrangThaiValue { get; set; }
        public int TongSoPhieu { get; set; }
        public int SoPhieuDaDuyet { get; set; }
    }
}

