using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class HopDongLaoDong : BaseAuditableEntity
    {
        public string SoHopDong { get; set; } = null!;
        public string Cccd { get; set; } = null!;
        public string LoaiHopDong { get; set; } = null!;
        public DateOnly NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }
        public decimal LuongCoBan { get; set; }
        public TrangThaiHopDong TrangThai { get; set; } = TrangThaiHopDong.HIEU_LUC;

        public NhanVien NhanVien { get; set; } = null!;
    }
}
