using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollManagementSystem.Domain.Models;
using System.Reflection.Emit;

namespace PayrollManagementSystem.Infrastructure.Persistence.Configurations
{
    public class NhanVienConfiguration : IEntityTypeConfiguration<NhanVien>
    {
        public void Configure(EntityTypeBuilder<NhanVien> builder)
        {
            builder.HasKey(e => e.Cccd);
            
            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.Cccd).HasMaxLength(20);
            builder.Property(e => e.HoTen).HasMaxLength(150);
            builder.Property(e => e.Sdt).HasMaxLength(15);
            builder.Property(e => e.Email).HasMaxLength(100);
            builder.Property(e => e.DanToc).HasMaxLength(50);
            builder.Property(e => e.DiaChi).HasMaxLength(255);
            builder.Property(e => e.ChuyenNganh).HasMaxLength(100);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50);
            builder.Property(e => e.SoBhxh).HasMaxLength(50);
            builder.Property(e => e.SoBhyt).HasMaxLength(50);
            builder.Property(e => e.SoTaiKhoan).HasMaxLength(50);
            builder.Property(e => e.TenNganHang).HasMaxLength(100);
            builder.Property(e => e.MaSoThue).HasMaxLength(50);
            builder.Property(e => e.IdPb).HasMaxLength(50);
            builder.Property(e => e.HeSoP2).HasPrecision(5, 2);

            builder.HasOne(d => d.PhongBan)
                .WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.IdPb)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.TaiKhoan)
                .WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.IdTaiKhoan)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class PhongBanConfiguration : IEntityTypeConfiguration<PhongBan>
    {
        public void Configure(EntityTypeBuilder<PhongBan> builder)
        {
            builder.HasKey(e => e.IdPb);
            builder.Property(e => e.IdPb).HasMaxLength(50);
            builder.Property(e => e.TenPb).HasMaxLength(100);
        }
    }

    public class QuyetDinhNhanSuConfiguration : IEntityTypeConfiguration<QuyetDinhNhanSu>
    {
        public void Configure(EntityTypeBuilder<QuyetDinhNhanSu> builder)
        {
            builder.HasKey(e => e.SoQuyetDinh);
            
            builder.HasIndex(e => new { e.Cccd, e.TrangThai, e.NgayHieuLuc });

            builder.Property(e => e.SoQuyetDinh).HasMaxLength(50);
            builder.Property(e => e.Cccd).HasMaxLength(20);
            builder.Property(e => e.LoaiQuyetDinh).HasMaxLength(100);
            builder.Property(e => e.IdBacLuongMoi).HasMaxLength(50);
            builder.Property(e => e.IdChucVuMoi).HasMaxLength(50);
            builder.Property(e => e.IdBacLuongCu).HasMaxLength(50);
            builder.Property(e => e.IdChucVuCu).HasMaxLength(50);
            builder.Property(e => e.NguoiKy).HasMaxLength(100);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50);

            builder.HasOne(d => d.NhanVien)
                .WithMany(p => p.QuyetDinhNhanSus)
                .HasForeignKey(d => d.Cccd)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.BacLuong)
                .WithMany(p => p.QuyetDinhNhanSus)
                .HasForeignKey(d => d.IdBacLuongMoi)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.ChucVuMoi)
                .WithMany(p => p.QuyetDinhNhanSus)
                .HasForeignKey(d => d.IdChucVuMoi)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class TaiKhoanConfiguration : IEntityTypeConfiguration<TaiKhoan>
    {
        public void Configure(EntityTypeBuilder<TaiKhoan> builder)
        {
            builder.HasKey(e => e.IdTaiKhoan);
            
            builder.HasIndex(e => e.TenTaiKhoan).IsUnique();

            builder.Property(e => e.IdTaiKhoan).ValueGeneratedOnAdd();
            builder.Property(e => e.TenTaiKhoan).HasMaxLength(50);
            builder.Property(e => e.MatKhauHash).HasMaxLength(255);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50);
            builder.Property(e => e.DangNhapLanDau).HasDefaultValue(true);
            builder.Property(e => e.RefreshTokenExpiryTime).HasColumnType("timestamp without time zone");

            builder.HasOne(d => d.VaiTro)
                .WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.IdVaiTro)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class VaiTroConfiguration : IEntityTypeConfiguration<VaiTro>
    {
        public void Configure(EntityTypeBuilder<VaiTro> builder)
        {
            builder.HasKey(e => e.IdVaiTro);
            builder.Property(e => e.IdVaiTro).ValueGeneratedOnAdd();
            builder.Property(e => e.TenVaiTro).HasMaxLength(100);
        }
    }

    public class ThanNhanConfiguration : IEntityTypeConfiguration<ThanNhan>
    {
        public void Configure(EntityTypeBuilder<ThanNhan> builder)
        {
            builder.HasKey(e => e.MaDinhDanh);
            builder.Property(e => e.MaDinhDanh).HasMaxLength(50);
            builder.Property(e => e.TenTn).HasMaxLength(150);
        }
    }

    public class ThanNhanNhanVienConfiguration : IEntityTypeConfiguration<ThanNhanNhanVien>
    {
        public void Configure(EntityTypeBuilder<ThanNhanNhanVien> builder)
        {
            builder.HasKey(e => new { e.Cccd, e.MaDinhDanh });

            builder.Property(e => e.Cccd).HasMaxLength(20);
            builder.Property(e => e.MaDinhDanh).HasMaxLength(50);

            builder.HasOne(d => d.NhanVien)
                .WithMany(p => p.ThanNhanNhanViens)
                .HasForeignKey(d => d.Cccd)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.ThanNhan)
                .WithMany(p => p.ThanNhanNhanViens)
                .HasForeignKey(d => d.MaDinhDanh)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.MoiQuanHe)
                .WithMany(p => p.ThanNhanNhanViens)
                .HasForeignKey(d => d.IdMqh)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class HopDongLaoDongConfiguration : IEntityTypeConfiguration<HopDongLaoDong>
    {
        public void Configure(EntityTypeBuilder<HopDongLaoDong> builder)
        {
            builder.HasKey(e => e.SoHopDong);
            
            builder.Property(e => e.SoHopDong).HasMaxLength(50);
            builder.Property(e => e.Cccd).HasMaxLength(20);
            builder.Property(e => e.LoaiHopDong).HasMaxLength(100);
            builder.Property(e => e.LuongCoBan).HasPrecision(18, 2);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50);

            builder.HasOne(d => d.NhanVien)
                .WithMany(p => p.HopDongLaoDongs)
                .HasForeignKey(d => d.Cccd)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class NhatKyTrangThaiConfiguration : IEntityTypeConfiguration<NhatKyTrangThai>
    {
        public void Configure(EntityTypeBuilder<NhatKyTrangThai> builder)
        {
            builder.HasKey(e => e.IdNhatKy);
            
            builder.Property(e => e.IdNhatKy).ValueGeneratedOnAdd().HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.Cccd).HasMaxLength(20);
            builder.Property(e => e.TrangThaiCu).HasConversion<string>().HasMaxLength(50);
            builder.Property(e => e.TrangThaiMoi).HasConversion<string>().HasMaxLength(50);
            builder.Property(e => e.LyDo).HasMaxLength(255);
            builder.Property(e => e.NgayThayDoi).HasColumnType("timestamp without time zone");
            builder.Property(e => e.NguoiThayDoi).HasMaxLength(150);

            builder.HasOne(d => d.NhanVien)
                .WithMany(p => p.NhatKyTrangThais)
                .HasForeignKey(d => d.Cccd)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
