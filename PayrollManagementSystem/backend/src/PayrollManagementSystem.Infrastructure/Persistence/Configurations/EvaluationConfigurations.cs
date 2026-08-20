using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Infrastructure.Persistence.Configurations
{
    public class KhungNangLucP2Configuration : IEntityTypeConfiguration<KhungNangLucP2>
    {
        public void Configure(EntityTypeBuilder<KhungNangLucP2> builder)
        {
            builder.HasKey(e => e.IdTieuChi);

            builder.Property(e => e.IdTieuChi).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.IdChucVu).IsRequired().HasMaxLength(50);
            builder.Property(e => e.TenNangLuc).IsRequired().HasMaxLength(150);
            builder.Property(e => e.MoTa).HasMaxLength(500);
            builder.Property(e => e.TyTrong).HasPrecision(5, 2);

            builder.HasOne(d => d.ChucVu)
                .WithMany(p => p.KhungNangLucs)
                .HasForeignKey(d => d.IdChucVu)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class KyDanhGiaConfiguration : IEntityTypeConfiguration<KyDanhGia>
    {
        public void Configure(EntityTypeBuilder<KyDanhGia> builder)
        {
            builder.HasKey(e => e.IdKyDanhGia);

            builder.Property(e => e.IdKyDanhGia).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.TenKyDanhGia).IsRequired().HasMaxLength(200);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50);
        }
    }

    public class PhieuDanhGiaNangLucConfiguration : IEntityTypeConfiguration<PhieuDanhGiaNangLuc>
    {
        public void Configure(EntityTypeBuilder<PhieuDanhGiaNangLuc> builder)
        {
            builder.HasKey(e => e.IdPhieu);

            builder.Property(e => e.IdPhieu).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(20);
            builder.Property(e => e.CccdQuanLy).HasMaxLength(20);
            builder.Property(e => e.DiemTongHop).HasPrecision(5, 2);
            builder.Property(e => e.HeSoP2).HasPrecision(5, 2);
            builder.Property(e => e.XepLoai).HasMaxLength(100);
            builder.Property(e => e.NhanXetChung).HasMaxLength(1000);
            builder.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50);

            builder.HasOne(d => d.KyDanhGia)
                .WithMany(p => p.PhieuDanhGias)
                .HasForeignKey(d => d.IdKyDanhGia)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.NhanVien)
                .WithMany()
                .HasForeignKey(d => d.CccdNhanVien)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.QuanLy)
                .WithMany()
                .HasForeignKey(d => d.CccdQuanLy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class ChiTietDanhGiaNangLucConfiguration : IEntityTypeConfiguration<ChiTietDanhGiaNangLuc>
    {
        public void Configure(EntityTypeBuilder<ChiTietDanhGiaNangLuc> builder)
        {
            builder.HasKey(e => e.IdChiTiet);

            builder.Property(e => e.IdChiTiet).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.NhanXetNhanVien).HasMaxLength(500);
            builder.Property(e => e.NhanXetQuanLy).HasMaxLength(500);

            builder.HasOne(d => d.PhieuDanhGia)
                .WithMany(p => p.ChiTietDanhGias)
                .HasForeignKey(d => d.IdPhieu)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.TieuChi)
                .WithMany()
                .HasForeignKey(d => d.IdTieuChi)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class MucQuyDoiP2Configuration : IEntityTypeConfiguration<MucQuyDoiP2>
    {
        public void Configure(EntityTypeBuilder<MucQuyDoiP2> builder)
        {
            builder.HasKey(e => e.IdQuyDoi);

            builder.Property(e => e.IdQuyDoi).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.XepLoai).IsRequired().HasMaxLength(100);
            builder.Property(e => e.DiemToiThieu).HasPrecision(5, 2);
            builder.Property(e => e.DiemToiDa).HasPrecision(5, 2);
            builder.Property(e => e.HeSoP2).HasPrecision(5, 2);
        }
    }
}
