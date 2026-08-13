using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Infrastructure.Persistence.Configurations
{
    public class LichLamViecConfiguration : IEntityTypeConfiguration<LichLamViec>
    {
        public void Configure(EntityTypeBuilder<LichLamViec> builder)
        {
            builder.HasKey(e => e.IdLich);

            builder.HasIndex(e => e.Nam)
                   .IsUnique()
                   .HasFilter("\"is_deleted\" = false");

            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.GhiChu).HasMaxLength(500);
        }
    }

    public class ChiTietLichLamViecConfiguration : IEntityTypeConfiguration<ChiTietLichLamViec>
    {
        public void Configure(EntityTypeBuilder<ChiTietLichLamViec> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Thu).HasMaxLength(20);
            builder.Property(e => e.LoaiNgay).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.TenNgayNghi).HasMaxLength(100);
            builder.Property(e => e.SoGioLam).HasColumnType("decimal(4,1)");

            builder.HasOne(e => e.LichLamViec)
                .WithMany(l => l.ChiTietLichLamViecs)
                .HasForeignKey(e => e.IdLich);

            builder.HasOne(e => e.CaLamViecMacDinh)
                .WithMany(c => c.ChiTietLichLamViecs)
                .HasForeignKey(e => e.IdCaLamViecMacDinh)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(e => new { e.IdLich, e.Ngay });
        }
    }

    public class CaLamViecConfiguration : IEntityTypeConfiguration<CaLamViec>
    {
        public void Configure(EntityTypeBuilder<CaLamViec> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.TenCa).IsRequired().HasMaxLength(150);
            builder.Property(e => e.XuyenNgay).HasDefaultValue(false);
            builder.Property(e => e.HeSoLuong).HasPrecision(5, 2).HasDefaultValue(1.0m);
            builder.Property(e => e.TrangThai).HasDefaultValue(true);
        }
    }

    public class KhungGioNghiConfiguration : IEntityTypeConfiguration<KhungGioNghi>
    {
        public void Configure(EntityTypeBuilder<KhungGioNghi> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.TenKhoangNghi).IsRequired().HasMaxLength(150);
            builder.Property(e => e.TinhVaoGioLam).HasDefaultValue(false);

            builder.HasOne(d => d.CaLamViec)
                  .WithMany(p => p.KhungGioNghis)
                  .HasForeignKey(d => d.IdCaLamViec)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class PhanCongCaConfiguration : IEntityTypeConfiguration<PhanCongCa>
    {
        public void Configure(EntityTypeBuilder<PhanCongCa> builder)
        {
            builder.HasKey(e => e.IdPhanCong);
            
            builder.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(20);
            builder.Property(e => e.GhiChu).HasMaxLength(255);

            builder.HasOne(e => e.NhanVien)
                .WithMany()
                .HasForeignKey(e => e.CccdNhanVien);

            builder.HasOne(e => e.CaLamViec)
                .WithMany(c => c.PhanCongCas)
                .HasForeignKey(e => e.IdCaLamViec);

            builder.HasIndex(e => new { e.CccdNhanVien, e.NgayLamViec }).IsUnique();
        }
    }

    public class ChamCongConfiguration : IEntityTypeConfiguration<ChamCong>
    {
        public void Configure(EntityTypeBuilder<ChamCong> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(20);
            builder.Property(e => e.SoGioLamThucTe).HasPrecision(5, 2);
            builder.Property(e => e.SoNgayCong).HasPrecision(5, 2);
            builder.Property(e => e.LoaiNgayCong).HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.IsNhapTay).HasDefaultValue(false);
            builder.Property(e => e.GhiChu).HasMaxLength(500);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(d => d.NhanVien)
                .WithMany()
                .HasForeignKey(d => d.CccdNhanVien)
                .HasPrincipalKey(n => n.Cccd)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.CccdNhanVien, e.NgayChamCong }).IsUnique();
        }
    }

    public class DonNghiConfiguration : IEntityTypeConfiguration<DonNghi>
    {
        public void Configure(EntityTypeBuilder<DonNghi> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(20);
            builder.Property(e => e.LoaiNghi).HasConversion<string>().HasMaxLength(30);
            builder.Property(e => e.SoNgayNghi).HasPrecision(5, 1);
            builder.Property(e => e.LyDo).IsRequired().HasMaxLength(500);
            builder.Property(e => e.TaiLieuDinhKem).HasMaxLength(500);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.CccdNguoiDuyet).HasMaxLength(20);
            builder.Property(e => e.LyDoTuChoi).HasMaxLength(500);
            builder.Property(e => e.NgayDuyet).HasColumnType("timestamp without time zone");

            builder.HasOne(d => d.NhanVien)
                .WithMany()
                .HasForeignKey(d => d.CccdNhanVien)
                .HasPrincipalKey(n => n.Cccd)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.NguoiDuyet)
                .WithMany()
                .HasForeignKey(d => d.CccdNguoiDuyet)
                .HasPrincipalKey(n => n.Cccd)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(e => new { e.CccdNhanVien, e.NgayBatDau, e.NgayKetThuc });
        }
    }

    public class NgayPhepNhanVienConfiguration : IEntityTypeConfiguration<NgayPhepNhanVien>
    {
        public void Configure(EntityTypeBuilder<NgayPhepNhanVien> builder)
        {
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(20);
            builder.Property(e => e.TongNgayPhep).HasPrecision(5, 1).HasDefaultValue(12m);
            builder.Property(e => e.DaSuDung).HasPrecision(5, 1).HasDefaultValue(0m);
            builder.Ignore(e => e.ConLai);

            builder.HasOne(d => d.NhanVien)
                .WithMany()
                .HasForeignKey(d => d.CccdNhanVien)
                .HasPrincipalKey(n => n.Cccd)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.CccdNhanVien, e.Nam }).IsUnique();
        }
    }
}
