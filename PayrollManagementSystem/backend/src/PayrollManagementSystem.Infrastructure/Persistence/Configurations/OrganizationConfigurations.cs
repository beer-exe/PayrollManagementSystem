using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollManagementSystem.Domain.Models;

namespace PayrollManagementSystem.Infrastructure.Persistence.Configurations
{
    public class BacLuongConfiguration : IEntityTypeConfiguration<BacLuong>
    {
        public void Configure(EntityTypeBuilder<BacLuong> builder)
        {
            builder.HasKey(e => e.IdBacLuong);

            builder.Property(e => e.IdBacLuong).HasMaxLength(50);
            builder.Property(e => e.IdNgachLuong).HasMaxLength(50);
            builder.Property(e => e.TenBacLuong).IsRequired().HasMaxLength(100);
            builder.Property(e => e.LuongP1).HasPrecision(18, 2);

            builder.HasOne(d => d.NgachLuong)
                .WithMany(p => p.BacLuongs)
                .HasForeignKey(d => d.IdNgachLuong)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ChucVuConfiguration : IEntityTypeConfiguration<ChucVu>
    {
        public void Configure(EntityTypeBuilder<ChucVu> builder)
        {
            builder.HasKey(e => e.IdChucVu);

            builder.Property(e => e.IdChucVu).HasMaxLength(50);
            builder.Property(e => e.IdNgachLuong).HasMaxLength(50);
            builder.Property(e => e.IdPhongBan).HasMaxLength(50);
            builder.Property(e => e.IdChucVuQuanLy).HasMaxLength(50);
            builder.Property(e => e.TenChucVu).HasMaxLength(100);
            builder.Property(e => e.MoTaCongViec).HasMaxLength(500);

            builder.HasOne(d => d.NgachLuong)
                .WithMany(p => p.ChucVus)
                .HasForeignKey(d => d.IdNgachLuong)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.PhongBan)
                .WithMany()
                .HasForeignKey(d => d.IdPhongBan)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.ChucVuQuanLy)
                .WithMany(p => p.ChucVuCapDuois)
                .HasForeignKey(d => d.IdChucVuQuanLy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class NgachLuongConfiguration : IEntityTypeConfiguration<NgachLuong>
    {
        public void Configure(EntityTypeBuilder<NgachLuong> builder)
        {
            builder.HasKey(e => e.IdNgachLuong);

            builder.Property(e => e.IdNgachLuong).HasMaxLength(50);
            builder.Property(e => e.TenNgachLuong).IsRequired().HasMaxLength(100);
            builder.Property(e => e.MoTa).HasMaxLength(500);
        }
    }

    public class MoiQuanHeConfiguration : IEntityTypeConfiguration<MoiQuanHe>
    {
        public void Configure(EntityTypeBuilder<MoiQuanHe> builder)
        {
            builder.HasKey(e => e.IdMqh);

            builder.Property(e => e.IdMqh).HasDefaultValueSql("gen_random_uuid()");
            builder.Property(e => e.TenQuanHe).HasMaxLength(100);
        }
    }
}
