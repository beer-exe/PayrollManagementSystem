using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Infrastructure.Persistence.Configurations
{
    public class KyKpiConfiguration : IEntityTypeConfiguration<KyKpi>
    {
        public void Configure(EntityTypeBuilder<KyKpi> builder)
        {
            builder.HasKey(e => e.IdKyKpi);
            builder.Property(e => e.TenKyKpi).IsRequired().HasMaxLength(100);

            builder.HasIndex(e => new { e.Thang, e.Nam })
                   .IsUnique()
                   .HasFilter("is_deleted = false");
        }
    }

    public class PhieuKpiConfiguration : IEntityTypeConfiguration<PhieuKpi>
    {
        public void Configure(EntityTypeBuilder<PhieuKpi> builder)
        {
            builder.HasKey(e => e.IdPhieuKpi);
            builder.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(12);
            builder.Property(e => e.CccdQuanLy).HasMaxLength(12);
            builder.Property(e => e.TongDiemKpi).HasPrecision(5, 2);
            builder.Property(e => e.HeSoP3).HasPrecision(5, 2);
            builder.Property(e => e.NhanXet).HasMaxLength(1000);

            builder.HasOne(e => e.KyKpi)
                .WithMany(e => e.PhieuKpis)
                .HasForeignKey(e => e.IdKyKpi)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.NhanVien)
                .WithMany()
                .HasForeignKey(e => e.CccdNhanVien)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.QuanLy)
                .WithMany()
                .HasForeignKey(e => e.CccdQuanLy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.IdKyKpi, e.CccdNhanVien })
                   .IsUnique()
                   .HasFilter("is_deleted = false");
        }
    }

    public class ChiTietKpiConfiguration : IEntityTypeConfiguration<ChiTietKpi>
    {
        public void Configure(EntityTypeBuilder<ChiTietKpi> builder)
        {
            builder.HasKey(e => e.IdChiTietKpi);
            builder.Property(e => e.MucTieu).IsRequired().HasMaxLength(500);
            builder.Property(e => e.DonViTinh).IsRequired().HasMaxLength(50);
            builder.Property(e => e.TrongSo).HasPrecision(5, 2);
            builder.Property(e => e.ChiTieu).HasPrecision(18, 2);
            builder.Property(e => e.ThucTe).HasPrecision(18, 2);
            builder.Property(e => e.TiLeHoanThanh).HasPrecision(5, 2);
            builder.Property(e => e.DiemKpi).HasPrecision(5, 2);
            builder.Property(e => e.LoaiTieuChi).HasConversion<string>().HasMaxLength(50);

            builder.HasOne(e => e.PhieuKpi)
                .WithMany(e => e.ChiTietKpis)
                .HasForeignKey(e => e.IdPhieuKpi)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
