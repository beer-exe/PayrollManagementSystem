using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Domain.Models;

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
        public virtual DbSet<MoiQuanHe> MoiQuanHes { get; set; }
        public virtual DbSet<NganHang> NganHangs { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<PhongBan> PhongBans { get; set; }
        public virtual DbSet<QuyetDinhNhanSu> QuyetDinhNhanSus { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<TaiKhoanNganHang> TaiKhoanNganHangs { get; set; }
        public virtual DbSet<ThanNhan> ThanNhans { get; set; }
        public virtual DbSet<ThanNhanNhanVien> TNhanNviens { get; set; }
        public virtual DbSet<VaiTro> VaiTros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pgcrypto");

            modelBuilder.Entity<BacLuong>(entity =>
            {
                entity.HasKey(e => e.IdBacLuong).HasName("bac_luongs_pkey");
                entity.ToTable("bac_luongs");

                entity.Property(e => e.IdBacLuong)
                    .HasMaxLength(50)
                    .HasColumnName("id_bac_luong");

                entity.Property(e => e.IdChucVu)
                    .HasMaxLength(50)
                    .HasColumnName("id_chuc_vu");

                entity.Property(e => e.LuongP1)
                    .HasPrecision(18, 2)
                    .HasColumnName("luong_p1");

                entity.Property(e => e.NgayApDung)
                    .HasColumnName("ngay_ap_dung");

                entity.HasOne(d => d.ChucVu)
                    .WithMany(p => p.BacLuongs)
                    .HasForeignKey(d => d.IdChucVu)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("bac_luongs_id_chuc_vu_fkey");
            });

            modelBuilder.Entity<ChucVu>(entity =>
            {
                entity.HasKey(e => e.IdChucVu).HasName("chuc_vus_pkey");
                entity.ToTable("chuc_vus");

                entity.Property(e => e.IdChucVu)
                    .HasMaxLength(50)
                    .HasColumnName("id_chuc_vu");

                entity.Property(e => e.TenChucVu)
                    .HasMaxLength(100)
                    .HasColumnName("ten_chuc_vu");
            });

            modelBuilder.Entity<MoiQuanHe>(entity =>
            {
                entity.HasKey(e => e.IdMqh).HasName("moi_quan_hes_pkey");
                entity.ToTable("moi_quan_hes");

                entity.Property(e => e.IdMqh)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id_mqh");

                entity.Property(e => e.TenQuanHe)
                    .HasMaxLength(100)
                    .HasColumnName("ten_quan_he");
            });

            modelBuilder.Entity<NganHang>(entity =>
            {
                entity.HasKey(e => e.IdNganHang).HasName("ngan_hangs_pkey");
                entity.ToTable("ngan_hangs");

                entity.Property(e => e.IdNganHang)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id_ngan_hang");

                entity.Property(e => e.TenNganHang)
                    .HasMaxLength(255)
                    .HasColumnName("ten_ngan_hang");
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.Cccd).HasName("nhan_viens_pkey");
                entity.ToTable("nhan_viens");

                entity.HasIndex(e => e.Email, "nhan_viens_email_key").IsUnique();

                entity.Property(e => e.Cccd)
                    .HasMaxLength(20)
                    .HasColumnName("cccd");
                entity.Property(e => e.HoTen)
                    .HasMaxLength(150)
                    .HasColumnName("ho_ten");
                entity.Property(e => e.GioiTinh).HasColumnName("gioi_tinh");
                entity.Property(e => e.Sdt).HasMaxLength(15).HasColumnName("sdt");
                entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
                entity.Property(e => e.NgaySinh).HasColumnName("ngay_sinh");
                entity.Property(e => e.DanToc).HasMaxLength(50).HasColumnName("dan_toc");
                entity.Property(e => e.DiaChi).HasMaxLength(255).HasColumnName("dia_chi");
                entity.Property(e => e.ChuyenNganh).HasMaxLength(100).HasColumnName("chuyen_nganh");
                entity.Property(e => e.NgayVaoLam).HasColumnName("ngay_vao_lam");
                entity.Property(e => e.NgayNghiViec).HasColumnName("ngay_nghi_viec");

                entity.Property(e => e.TrangThai)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .HasColumnName("trang_thai");

                entity.Property(e => e.SoBhxh).HasMaxLength(50).HasColumnName("so_bhxh");
                entity.Property(e => e.SoBhyt).HasMaxLength(50).HasColumnName("so_bhyt");
                entity.Property(e => e.IdPb).HasMaxLength(50).HasColumnName("id_pb");
                entity.Property(e => e.IdTaiKhoan).HasColumnName("id_tai_khoan");

                entity.HasOne(d => d.PhongBan)
                    .WithMany(p => p.NhanViens)
                    .HasForeignKey(d => d.IdPb)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("nhan_viens_id_pb_fkey");

                entity.HasOne(d => d.TaiKhoan)
                    .WithOne(p => p.NhanVien)
                    .HasForeignKey<NhanVien>(d => d.IdTaiKhoan)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("nhan_viens_id_tai_khoan_fkey");
            });

            modelBuilder.Entity<PhongBan>(entity =>
            {
                entity.HasKey(e => e.IdPb).HasName("phong_bans_pkey");
                entity.ToTable("phong_bans");

                entity.Property(e => e.IdPb)
                    .HasMaxLength(50)
                    .HasColumnName("id_pb");
                entity.Property(e => e.TenPb)
                    .HasMaxLength(100)
                    .HasColumnName("ten_pb");
            });

            modelBuilder.Entity<QuyetDinhNhanSu>(entity =>
            {
                entity.HasKey(e => e.SoQuyetDinh).HasName("quyet_dinh_nhan_sus_pkey");
                entity.ToTable("quyet_dinh_nhan_sus");

                entity.Property(e => e.SoQuyetDinh)
                    .HasMaxLength(50)
                    .HasColumnName("so_quyet_dinh");
                entity.Property(e => e.Cccd)
                    .HasMaxLength(20)
                    .HasColumnName("cccd");
                entity.Property(e => e.LoaiQuyetDinh)
                    .HasMaxLength(100)
                    .HasColumnName("loai_quyet_dinh");
                entity.Property(e => e.IdBacLuongMoi)
                    .HasMaxLength(50)
                    .HasColumnName("id_bac_luong_moi");
                entity.Property(e => e.IdChucVuMoi)
                    .HasMaxLength(50)
                    .HasColumnName("id_chuc_vu_moi");
                entity.Property(e => e.NgayHieuLuc).HasColumnName("ngay_hieu_luc");
                entity.Property(e => e.NgayHetHan).HasColumnName("ngay_het_han");
                entity.Property(e => e.NguoiKy).HasMaxLength(100).HasColumnName("nguoi_ky");

                entity.Property(e => e.TrangThai)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .HasColumnName("trang_thai");

                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.QuyetDinhNhanSus)
                    .HasForeignKey(d => d.Cccd)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("quyet_dinh_nhan_sus_cccd_fkey");

                entity.HasOne(d => d.BacLuong)
                    .WithMany(p => p.QuyetDinhNhanSus)
                    .HasForeignKey(d => d.IdBacLuongMoi)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("quyet_dinh_nhan_sus_id_bac_luong_moi_fkey");
            });

            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasKey(e => e.IdTaiKhoan).HasName("tai_khoans_pkey");
                entity.ToTable("tai_khoans");

                entity.HasIndex(e => e.TenTaiKhoan, "tai_khoans_ten_tai_khoan_key").IsUnique();

                entity.Property(e => e.IdTaiKhoan)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id_tai_khoan");

                entity.Property(e => e.TenTaiKhoan)
                    .HasMaxLength(50)
                    .HasColumnName("ten_tai_khoan");

                entity.Property(e => e.MatKhauHash)
                    .HasMaxLength(255)
                    .HasColumnName("mat_khau_hash");

                entity.Property(e => e.TrangThai)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .HasColumnName("trang_thai");

                entity.Property(e => e.DangNhapLanDau)
                    .HasDefaultValue(true)
                    .HasColumnName("dang_nhap_lan_dau");

                entity.Property(e => e.IdVaiTro).HasColumnName("id_vai_tro");

                entity.Property(e => e.RefreshToken)
                    .HasColumnName("refresh_token");

                entity.Property(e => e.RefreshTokenExpiryTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("refresh_token_expiry_time");

                entity.HasOne(d => d.VaiTro)
                    .WithMany(p => p.TaiKhoans)
                    .HasForeignKey(d => d.IdVaiTro)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("tai_khoans_id_vai_tro_fkey");
            });

            modelBuilder.Entity<TaiKhoanNganHang>(entity =>
            {
                entity.HasKey(e => e.Stk).HasName("tai_khoan_ngan_hangs_pkey");
                entity.ToTable("tai_khoan_ngan_hangs");

                entity.Property(e => e.Stk)
                    .HasMaxLength(50)
                    .HasColumnName("stk");
                entity.Property(e => e.ChiNhanh)
                    .HasMaxLength(100)
                    .HasColumnName("chi_nhanh");
                entity.Property(e => e.NgayMoThe).HasColumnName("ngay_mo_the");
                entity.Property(e => e.TrangThai).HasMaxLength(50).HasColumnName("trang_thai");
                entity.Property(e => e.IdNganHang).HasColumnName("id_ngan_hang");
                entity.Property(e => e.Cccd)
                    .HasMaxLength(20)
                    .HasColumnName("cccd");

                entity.HasOne(d => d.NganHang)
                    .WithMany(p => p.TaiKhoanNganHangs)
                    .HasForeignKey(d => d.IdNganHang)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("tai_khoan_ngan_hangs_id_ngan_hang_fkey");

                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.TaiKhoanNganHangs)
                    .HasForeignKey(d => d.Cccd)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("tai_khoan_ngan_hangs_cccd_fkey");
            });

            modelBuilder.Entity<ThanNhan>(entity =>
            {
                entity.HasKey(e => e.MaDinhDanh).HasName("than_nhans_pkey");
                entity.ToTable("than_nhans");

                entity.Property(e => e.MaDinhDanh)
                    .HasMaxLength(50)
                    .HasColumnName("ma_dinh_danh");

                entity.Property(e => e.TenTn)
                    .HasMaxLength(150)
                    .HasColumnName("ten_tn");

                entity.Property(e => e.NgaySinh).HasColumnName("ngay_sinh");
            });

            modelBuilder.Entity<ThanNhanNhanVien>(entity =>
            {
                entity.HasKey(e => new { e.Cccd, e.MaDinhDanh }).HasName("than_nhan_nhan_vien_pkey");
                entity.ToTable("than_nhan_nhan_vien");

                entity.Property(e => e.Cccd)
                    .HasMaxLength(20)
                    .HasColumnName("cccd");
                entity.Property(e => e.MaDinhDanh)
                    .HasMaxLength(50)
                    .HasColumnName("ma_dinh_danh");
                entity.Property(e => e.IdMqh).HasColumnName("id_mqh");

                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.ThanNhanNhanViens)
                    .HasForeignKey(d => d.Cccd)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("than_nhan_nhan_vien_cccd_fkey");

                entity.HasOne(d => d.ThanNhan)
                    .WithMany(p => p.ThanNhanNhanViens)
                    .HasForeignKey(d => d.MaDinhDanh)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("than_nhan_nhan_vien_ma_dinh_danh_fkey");

                entity.HasOne(d => d.MoiQuanHe)
                    .WithMany(p => p.ThanNhanNhanViens)
                    .HasForeignKey(d => d.IdMqh)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("than_nhan_nhan_vien_id_mqh_fkey");
            });

            modelBuilder.Entity<VaiTro>(entity =>
            {
                entity.HasKey(e => e.IdVaiTro).HasName("vai_tros_pkey");
                entity.ToTable("vai_tros");

                entity.Property(e => e.IdVaiTro)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id_vai_tro");

                entity.Property(e => e.TenVaiTro)
                    .HasMaxLength(100)
                    .HasColumnName("ten_vai_tro");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
