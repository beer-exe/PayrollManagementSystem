using PayrollManagementSystem.Domain.Common;

namespace PayrollManagementSystem.Domain.Models
{
    public class CaLamViec : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenCa { get; set; } = null!;
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
        public bool XuyenNgay { get; set; }
        public decimal HeSoLuong { get; set; } = 1.0m;
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public ICollection<KhungGioNghi> KhungGioNghis { get; set; } = new List<KhungGioNghi>();
        public ICollection<ChiTietLichLamViec> ChiTietLichLamViecs { get; set; } = new List<ChiTietLichLamViec>();
        public ICollection<PhanCongCa> PhanCongCas { get; set; } = new List<PhanCongCa>();

        public decimal CalculateWorkingHours()
        {
            var diff = GioKetThuc - GioBatDau;
            if (XuyenNgay || diff.TotalHours < 0)
            {
                diff = diff.Add(TimeSpan.FromHours(24));
            }
            
            decimal totalHours = (decimal)diff.TotalHours;

            if (KhungGioNghis != null)
            {
                foreach (var breakTime in KhungGioNghis)
                {
                    if (!breakTime.TinhVaoGioLam && !breakTime.IsDeleted)
                    {
                        var breakDiff = breakTime.GioKetThuc - breakTime.GioBatDau;
                        if (breakDiff.TotalHours < 0)
                        {
                            breakDiff = breakDiff.Add(TimeSpan.FromHours(24));
                        }
                        totalHours -= (decimal)breakDiff.TotalHours;
                    }
                }
            }

            return totalHours > 0 ? totalHours : 0;
        }
    }
}
