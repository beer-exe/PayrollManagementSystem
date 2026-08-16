namespace PayrollManagementSystem.Application.Features.Kpi.DTOs
{
    public class ChiTietKpiDto
    {
        public Guid IdChiTietKpi { get; set; }
        public Guid IdPhieuKpi { get; set; }
        public string MucTieu { get; set; } = null!;
        public string DonViTinh { get; set; } = null!;
        public decimal TrongSo { get; set; }
        public decimal ChiTieu { get; set; }
        public decimal ThucTe { get; set; }
        public decimal TiLeHoanThanh { get; set; }
        public decimal DiemKpi { get; set; }
    }
}

