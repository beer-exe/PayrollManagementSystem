using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Infrastructure.Persistence.Configurations
{
    public class KyLuongConfiguration : IEntityTypeConfiguration<KyLuong>
    {
        public void Configure(EntityTypeBuilder<KyLuong> builder)
        {
            builder.HasKey(e => e.IdKyLuong);
            
            builder.Property(e => e.TenKyLuong).HasMaxLength(150);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50);

            builder.HasIndex(e => new { e.Thang, e.Nam })
                   .IsUnique()
                   .HasFilter("is_deleted = false");
        }
    }

    public class BangLuongConfiguration : IEntityTypeConfiguration<BangLuong>
    {
        public void Configure(EntityTypeBuilder<BangLuong> builder)
        {
            builder.HasKey(e => e.IdBangLuong);
            
            builder.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(20);
            builder.Property(e => e.P1).HasPrecision(18, 2);
            builder.Property(e => e.HeSoP2).HasPrecision(5, 2);
            builder.Property(e => e.HeSoP3).HasPrecision(5, 2);
            builder.Property(e => e.NgayCongChuan).HasPrecision(5, 2);
            builder.Property(e => e.NgayCongThucTe).HasPrecision(5, 2);
            builder.Property(e => e.LuongThoiGian).HasPrecision(18, 2);
            builder.Property(e => e.LuongHieuSuatP3).HasPrecision(18, 2);
            builder.Property(e => e.PhuCap).HasPrecision(18, 2);
            builder.Property(e => e.Thuong).HasPrecision(18, 2);
            builder.Property(e => e.TangCa).HasPrecision(18, 2);
            builder.Property(e => e.Phat).HasPrecision(18, 2);
            builder.Property(e => e.KhauTru).HasPrecision(18, 2);
            builder.Property(e => e.TruThue).HasPrecision(18, 2);
            builder.Property(e => e.TongThuNhap).HasPrecision(18, 2);
            builder.Property(e => e.ThucLinh).HasPrecision(18, 2);
            builder.Property(e => e.GhiChu).HasMaxLength(500);

            builder.HasOne(d => d.KyLuong)
                .WithMany(p => p.BangLuongs)
                .HasForeignKey(d => d.IdKyLuong)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.NhanVien)
                .WithMany()
                .HasForeignKey(d => d.CccdNhanVien)
                .HasPrincipalKey(p => p.Cccd)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.IdKyLuong, e.CccdNhanVien })
                   .IsUnique()
                   .HasFilter("is_deleted = false");
        }
    }

    public class KhoanKhauTruConfiguration : IEntityTypeConfiguration<KhoanKhauTru>
    {
        public void Configure(EntityTypeBuilder<KhoanKhauTru> builder)
        {
            builder.HasKey(e => e.IdKhoanKhauTru);
            
            builder.Property(e => e.TenKhoanKhauTru).IsRequired().HasMaxLength(200);
            builder.Property(e => e.LoaiCongThuc).HasConversion<string>().HasMaxLength(50);
            builder.Property(e => e.GiaTri).HasPrecision(18, 4);
            builder.Property(e => e.GhiChu).HasMaxLength(500);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);

            builder.HasIndex(e => e.TenKhoanKhauTru)
                   .IsUnique()
                   .HasFilter("is_deleted = false");
        }
    }

    public class BacThueConfiguration : IEntityTypeConfiguration<BacThue>
    {
        public void Configure(EntityTypeBuilder<BacThue> builder)
        {
            builder.HasKey(e => e.IdBacThue);
            
            builder.Property(e => e.TuGia).HasPrecision(18, 2);
            builder.Property(e => e.DenGia).HasPrecision(18, 2);
            builder.Property(e => e.ThueSuat).HasPrecision(5, 2);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        }
    }

    public class CauHinhGiamTruConfiguration : IEntityTypeConfiguration<CauHinhGiamTru>
    {
        public void Configure(EntityTypeBuilder<CauHinhGiamTru> builder)
        {
            builder.HasKey(e => e.IdCauHinhGiamTru);
            
            builder.Property(e => e.GiamTruBanThan).HasPrecision(18, 2);
            builder.Property(e => e.GiamTruNguoiPhuThuoc).HasPrecision(18, 2);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.GhiChu).HasMaxLength(500);
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        }
    }

    public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> builder)
        {
            builder.ToTable("systemlogs", t => t.ExcludeFromMigrations());
            builder.HasNoKey();
            // Không map chi tiết các property để tránh nhầm lẫn (Dùng Raw SQL qua ADO.NET trong Repository)
        }
    }
}
