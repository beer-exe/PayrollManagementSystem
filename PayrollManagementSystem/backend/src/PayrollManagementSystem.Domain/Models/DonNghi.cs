using PayrollManagementSystem.Domain.Common;
using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class DonNghi : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string CccdNhanVien { get; set; } = null!;

        public LoaiNghi LoaiNghi { get; set; }

        public DateOnly NgayBatDau { get; set; }

        public DateOnly NgayKetThuc { get; set; }

        public decimal SoNgayNghi { get; set; }

        public string LyDo { get; set; } = null!;

        public string? TaiLieuDinhKem { get; set; }

        public TrangThaiDonNghi TrangThai { get; set; } = TrangThaiDonNghi.CHO_DUYET;

        public string? CccdNguoiDuyet { get; set; }

        public string? LyDoTuChoi { get; set; }

        public DateTime? NgayDuyet { get; set; }

        // Navigation properties
        public NhanVien NhanVien { get; set; } = null!;
        public NhanVien? NguoiDuyet { get; set; }
    }
}
