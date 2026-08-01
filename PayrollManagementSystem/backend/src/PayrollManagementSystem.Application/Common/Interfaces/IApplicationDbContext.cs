using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<BacLuong> BacLuongs { get; set; }
        DbSet<ChucVu> ChucVus { get; set; }
        DbSet<NgachLuong> NgachLuongs { get; set; }
        DbSet<MoiQuanHe> MoiQuanHes { get; set; }

        DbSet<NhanVien> NhanViens { get; set; }
        DbSet<PhongBan> PhongBans { get; set; }
        DbSet<QuyetDinhNhanSu> QuyetDinhNhanSus { get; set; }
        DbSet<TaiKhoan> TaiKhoans { get; set; }

        DbSet<ThanNhan> ThanNhans { get; set; }
        DbSet<ThanNhanNhanVien> TNhanNviens { get; set; }
        DbSet<VaiTro> VaiTros { get; set; }
        DbSet<HopDongLaoDong> HopDongLaoDongs { get; set; }

        DbSet<NhatKyTrangThai> NhatKyTrangThais { get; set; }
        DbSet<KhungNangLucP2> KhungNangLucP2s { get; set; }

        DbSet<KyDanhGia> KyDanhGias { get; set; }
        DbSet<PhieuDanhGiaNangLuc> PhieuDanhGiaNangLucs { get; set; }
        DbSet<ChiTietDanhGiaNangLuc> ChiTietDanhGiaNangLucs { get; set; }
        DbSet<MucQuyDoiP2> MucQuyDoiP2s { get; set; }

        DbSet<LichLamViec> LichLamViecs { get; set; }
        DbSet<ChiTietLichLamViec> ChiTietLichLamViecs { get; set; }

        DbSet<CaLamViec> CaLamViecs { get; set; }
        DbSet<KhungGioNghi> KhungGioNghis { get; set; }
        DbSet<PhanCongCa> PhanCongCas { get; set; }

        DbSet<ChamCong> ChamCongs { get; set; }

        DbSet<DonNghi> DonNghis { get; set; }
        DbSet<NgayPhepNhanVien> NgayPhepNhanViens { get; set; }

        DbSet<KyLuong> KyLuongs { get; set; }
        DbSet<BangLuong> BangLuongs { get; set; }

        DbSet<KhoanKhauTru> KhoanKhauTrus { get; set; }
        DbSet<BacThue> BacThues { get; set; }
        DbSet<CauHinhGiamTru> CauHinhGiamTrus { get; set; }

        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        void SoftRemove<TEntity>(TEntity entity) where TEntity : PayrollManagementSystem.Domain.Common.BaseAuditableEntity;
        void SoftRemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : PayrollManagementSystem.Domain.Common.BaseAuditableEntity;
    }
}
