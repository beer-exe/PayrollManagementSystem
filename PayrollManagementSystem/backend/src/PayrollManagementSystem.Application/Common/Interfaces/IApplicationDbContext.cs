using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<BacLuong> BacLuongs { get; set; }
        DbSet<ChucVu> ChucVus { get; set; }
        DbSet<MoiQuanHe> MoiQuanHes { get; set; }
        DbSet<NganHang> NganHangs { get; set; }
        DbSet<NhanVien> NhanViens { get; set; }
        DbSet<PhongBan> PhongBans { get; set; }
        DbSet<QuyetDinhNhanSu> QuyetDinhNhanSus { get; set; }
        DbSet<TaiKhoan> TaiKhoans { get; set; }
        DbSet<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; }
        DbSet<ThanNhan> ThanNhans { get; set; }
        DbSet<ThanNhanNhanVien> TNhanNviens { get; set; }
        DbSet<VaiTro> VaiTros { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
