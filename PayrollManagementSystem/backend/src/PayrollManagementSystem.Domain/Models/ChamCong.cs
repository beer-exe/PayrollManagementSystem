using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class ChamCong : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string CccdNhanVien { get; set; } = null!;

        public DateOnly NgayChamCong { get; set; }

        public TimeOnly? GioVao { get; set; }

        public TimeOnly? GioRa { get; set; }

        public decimal SoGioLamThucTe { get; set; }

        public decimal SoNgayCong { get; set; }

        public LoaiNgayCong LoaiNgayCong { get; set; } = LoaiNgayCong.LAM_DU_CA;

        public bool IsNhapTay { get; set; } = false;

        public int SoPhutDiTre { get; set; } = 0;

        public int SoPhutVeSom { get; set; } = 0;

        public string? GhiChu { get; set; }

        public TrangThaiChamCong TrangThai { get; set; } = TrangThaiChamCong.CHUA_XAC_NHAN;

        // Navigation
        public Guid? IdKyChamCong { get; set; }
        public KyChamCong? KyChamCong { get; set; }
        public NhanVien NhanVien { get; set; } = null!;
    }
}
