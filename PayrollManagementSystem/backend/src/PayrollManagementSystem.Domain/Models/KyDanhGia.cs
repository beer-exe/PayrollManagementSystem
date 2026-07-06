using PayrollManagementSystem.Domain.Enums;

namespace PayrollManagementSystem.Domain.Models
{
    public class KyDanhGia
    {
        public Guid IdKyDanhGia { get; set; }
        public string TenKyDanhGia { get; set; } = null!;
        public int Nam { get; set; }
        public DateOnly NgayBatDau { get; set; }
        public DateOnly NgayKetThuc { get; set; }
        public TrangThaiKyDanhGia TrangThai { get; set; } = TrangThaiKyDanhGia.KHOI_TAO;

        public ICollection<PhieuDanhGiaNangLuc> PhieuDanhGias { get; set; } = new List<PhieuDanhGiaNangLuc>();
    }
}
