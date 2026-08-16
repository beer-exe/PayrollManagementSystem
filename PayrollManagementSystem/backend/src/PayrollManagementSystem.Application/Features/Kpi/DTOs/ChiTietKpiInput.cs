namespace PayrollManagementSystem.Application.Features.Kpi.DTOs
{
    public class ChiTietKpiInput
    {
        public Guid? IdChiTietKpi { get; set; }
        public string MucTieu { get; set; } = null!;
        public string DonViTinh { get; set; } = null!;
        public decimal TrongSo { get; set; }
        public decimal ChiTieu { get; set; }
        public decimal ThucTe { get; set; }
    }
}

