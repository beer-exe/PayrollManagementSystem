using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Models;
using System.Reflection;

namespace PayrollManagementSystem.Infrastructure.Persistence
{

    public partial class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public virtual DbSet<BacLuong> BacLuongs { get; set; }
        public virtual DbSet<ChucVu> ChucVus { get; set; }
        public virtual DbSet<NgachLuong> NgachLuongs { get; set; }
        public virtual DbSet<MoiQuanHe> MoiQuanHes { get; set; }

        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<PhongBan> PhongBans { get; set; }
        public virtual DbSet<QuyetDinhNhanSu> QuyetDinhNhanSus { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

        public virtual DbSet<ThanNhan> ThanNhans { get; set; }
        public virtual DbSet<ThanNhanNhanVien> TNhanNviens { get; set; }
        public virtual DbSet<VaiTro> VaiTros { get; set; }
        public virtual DbSet<HopDongLaoDong> HopDongLaoDongs { get; set; }

        public virtual DbSet<NhatKyTrangThai> NhatKyTrangThais { get; set; }
        public virtual DbSet<KhungNangLucP2> KhungNangLucP2s { get; set; }

        public virtual DbSet<KyDanhGia> KyDanhGias { get; set; }
        public virtual DbSet<PhieuDanhGiaNangLuc> PhieuDanhGiaNangLucs { get; set; }
        public virtual DbSet<ChiTietDanhGiaNangLuc> ChiTietDanhGiaNangLucs { get; set; }
        public virtual DbSet<MucQuyDoiP2> MucQuyDoiP2s { get; set; }

        public virtual DbSet<LichLamViec> LichLamViecs { get; set; }
        public virtual DbSet<ChiTietLichLamViec> ChiTietLichLamViecs { get; set; }

        public virtual DbSet<CaLamViec> CaLamViecs { get; set; }
        public virtual DbSet<KhungGioNghi> KhungGioNghis { get; set; }
        public virtual DbSet<PhanCongCa> PhanCongCas { get; set; }

        public virtual DbSet<ChamCong> ChamCongs { get; set; }

        public virtual DbSet<DonNghi> DonNghis { get; set; }
        public virtual DbSet<NgayPhepNhanVien> NgayPhepNhanViens { get; set; }

        public virtual DbSet<KyLuong> KyLuongs { get; set; }
        public virtual DbSet<BangLuong> BangLuongs { get; set; }

        public virtual DbSet<KhoanKhauTru> KhoanKhauTrus { get; set; }
        public virtual DbSet<BacThue> BacThues { get; set; }
        public virtual DbSet<CauHinhGiamTru> CauHinhGiamTrus { get; set; }

        public virtual DbSet<SystemLog> SystemLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pgcrypto");

            // Apply all configurations from IEntityTypeConfiguration classes in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Global Query Filter for Soft Delete
            // Must be applied AFTER ApplyConfigurationsFromAssembly so it doesn't get overwritten
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(PayrollManagementSystem.Domain.Common.BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "p");
                    var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
                    var body = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(System.Linq.Expressions.Expression.Lambda(body, parameter));
                }
            }
        }

        public void SoftRemove<TEntity>(TEntity entity) where TEntity : PayrollManagementSystem.Domain.Common.BaseAuditableEntity
        {
            entity.IsDeleted = true;
            Entry(entity).State = EntityState.Modified;
        }

        public void SoftRemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : PayrollManagementSystem.Domain.Common.BaseAuditableEntity
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                Entry(entity).State = EntityState.Modified;
            }
        }
    }
}
