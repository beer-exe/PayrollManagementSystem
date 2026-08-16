namespace PayrollManagementSystem.Application.Features.Kpi.DTOs
{
    public class PhieuKpiDetailDto
    {
        public Guid IdPhieuKpi { get; set; }
        public Guid IdKyKpi { get; set; }
        public string TenKyKpi { get; set; } = null!;
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string CccdNhanVien { get; set; } = null!;
        public string TenNhanVien { get; set; } = null!;
        public string? CccdQuanLy { get; set; }
        public string? TenQuanLy { get; set; }
        public decimal TongDiemKpi { get; set; }
        public decimal HeSoP3 { get; set; }
        public string? NhanXet { get; set; }
        public string TrangThai { get; set; } = null!;
        public int TrangThaiValue { get; set; }
        public bool CanManage { get; set; }
        
        public List<ChiTietKpiDto> ChiTietKpis { get; set; } = new();
    }
}

