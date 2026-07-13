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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("pgcrypto");

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

            modelBuilder.Entity<NgachLuong>(entity =>
            {
                entity.HasKey(e => e.IdNgachLuong).HasName("ngach_luongs_pkey");
                entity.ToTable("ngach_luongs");

                entity.Property(e => e.IdNgachLuong)
                    .HasMaxLength(50)
                    .HasColumnName("id_ngach_luong");

                entity.Property(e => e.TenNgachLuong)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("ten_ngach_luong");

                entity.Property(e => e.MoTa)
                    .HasMaxLength(500)
                    .HasColumnName("mo_ta");
            });

            modelBuilder.Entity<BacLuong>(entity =>
            {
                entity.HasKey(e => e.IdBacLuong).HasName("bac_luongs_pkey");
                entity.ToTable("bac_luongs");

                entity.Property(e => e.IdBacLuong)
                    .HasMaxLength(50)
                    .HasColumnName("id_bac_luong");

                entity.Property(e => e.IdNgachLuong)
                    .HasMaxLength(50)
                    .HasColumnName("id_ngach_luong");

                entity.Property(e => e.TenBacLuong)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("ten_bac_luong");

                entity.Property(e => e.LuongP1)
                    .HasPrecision(18, 2)
                    .HasColumnName("luong_p1");

                entity.Property(e => e.NgayApDung)
                    .HasColumnName("ngay_ap_dung");

                entity.Property(e => e.NgayKetThuc)
                    .HasColumnName("ngay_ket_thuc");

                entity.HasOne(d => d.NgachLuong)
                    .WithMany(p => p.BacLuongs)
                    .HasForeignKey(d => d.IdNgachLuong)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("bac_luongs_id_ngach_luong_fkey");
            });

            modelBuilder.Entity<ChucVu>(entity =>
            {
                entity.HasKey(e => e.IdChucVu).HasName("chuc_vus_pkey");
                entity.ToTable("chuc_vus");

                entity.Property(e => e.IdChucVu)
                    .HasMaxLength(50)
                    .HasColumnName("id_chuc_vu");

                entity.Property(e => e.IdNgachLuong)
                    .HasMaxLength(50)
                    .HasColumnName("id_ngach_luong");

                entity.Property(e => e.IdPhongBan)
                    .HasMaxLength(50)
                    .HasColumnName("id_phong_ban");

                entity.Property(e => e.IdChucVuQuanLy)
                    .HasMaxLength(50)
                    .HasColumnName("id_chuc_vu_quan_ly");

                entity.Property(e => e.TenChucVu)
                    .HasMaxLength(100)
                    .HasColumnName("ten_chuc_vu");

                entity.Property(e => e.MoTaCongViec)
                    .HasMaxLength(500)
                    .HasColumnName("mo_ta_cong_viec");

                entity.HasOne(d => d.NgachLuong)
                    .WithMany(p => p.ChucVus)
                    .HasForeignKey(d => d.IdNgachLuong)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("chuc_vus_id_ngach_luong_fkey");

                entity.HasOne(d => d.PhongBan)
                    .WithMany()
                    .HasForeignKey(d => d.IdPhongBan)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("chuc_vus_id_phong_ban_fkey");

                entity.HasOne(d => d.ChucVuQuanLy)
                    .WithMany(p => p.ChucVuCapDuois)
                    .HasForeignKey(d => d.IdChucVuQuanLy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("chuc_vus_id_chuc_vu_quan_ly_fkey");
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
                entity.Property(e => e.SoTaiKhoan).HasMaxLength(50).HasColumnName("so_tai_khoan");
                entity.Property(e => e.TenNganHang).HasMaxLength(100).HasColumnName("ten_ngan_hang");
                entity.Property(e => e.MaSoThue).HasMaxLength(50).HasColumnName("ma_so_thue");
                entity.Property(e => e.IdPb).HasMaxLength(50).HasColumnName("id_pb");
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

                entity.Property(e => e.HeSoP2).HasPrecision(5, 2).HasColumnName("he_so_p2");
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

            modelBuilder.Entity<HopDongLaoDong>(entity =>
            {
                entity.HasKey(e => e.SoHopDong).HasName("hop_dong_lao_dongs_pkey");
                entity.ToTable("hop_dong_lao_dongs");

                entity.Property(e => e.SoHopDong).HasMaxLength(50).HasColumnName("so_hop_dong");
                entity.Property(e => e.Cccd).HasMaxLength(20).HasColumnName("cccd");
                entity.Property(e => e.LoaiHopDong).HasMaxLength(100).HasColumnName("loai_hop_dong");
                entity.Property(e => e.NgayBatDau).HasColumnName("ngay_bat_dau");
                entity.Property(e => e.NgayKetThuc).HasColumnName("ngay_ket_thuc");
                entity.Property(e => e.LuongCoBan).HasPrecision(18, 2).HasColumnName("luong_co_ban");

                entity.Property(e => e.TrangThai)
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .HasColumnName("trang_thai");

                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.HopDongLaoDongs)
                    .HasForeignKey(d => d.Cccd)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("hop_dong_lao_dongs_cccd_fkey");
            });



            modelBuilder.Entity<NhatKyTrangThai>(entity =>
            {
                entity.HasKey(e => e.IdNhatKy).HasName("nhat_ky_trang_thais_pkey");
                entity.ToTable("nhat_ky_trang_thais");

                entity.Property(e => e.IdNhatKy).ValueGeneratedOnAdd().HasColumnName("id_nhat_ky").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Cccd).HasMaxLength(20).HasColumnName("cccd");
                entity.Property(e => e.TrangThaiCu).HasConversion<string>().HasMaxLength(50).HasColumnName("trang_thai_cu");
                entity.Property(e => e.TrangThaiMoi).HasConversion<string>().HasMaxLength(50).HasColumnName("trang_thai_moi");
                entity.Property(e => e.LyDo).HasMaxLength(255).HasColumnName("ly_do");
                entity.Property(e => e.NgayThayDoi).HasColumnType("timestamp without time zone").HasColumnName("ngay_thay_doi");
                entity.Property(e => e.NguoiThayDoi).HasMaxLength(150).HasColumnName("nguoi_thay_doi");

                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.NhatKyTrangThais)
                    .HasForeignKey(d => d.Cccd)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("nhat_ky_trang_thais_cccd_fkey");
            });

            modelBuilder.Entity<KhungNangLucP2>(entity =>
            {
                entity.ToTable("khung_nang_luc_p2");
                entity.HasKey(e => e.IdTieuChi).HasName("khung_nang_luc_p2_pkey");

                entity.Property(e => e.IdTieuChi).HasColumnName("id_tieu_chi").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.IdChucVu).IsRequired().HasMaxLength(50).HasColumnName("id_chuc_vu");
                entity.Property(e => e.TenNangLuc).IsRequired().HasMaxLength(150).HasColumnName("ten_nang_luc");
                entity.Property(e => e.MoTa).HasMaxLength(500).HasColumnName("mo_ta");
                entity.Property(e => e.TyTrong).HasPrecision(5, 2).HasColumnName("ty_trong");

                entity.HasOne(d => d.ChucVu)
                      .WithMany(p => p.KhungNangLucs)
                      .HasForeignKey(d => d.IdChucVu)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("khung_nang_luc_id_chuc_vu_fkey");
            });



            modelBuilder.Entity<KyDanhGia>(entity =>
            {
                entity.ToTable("ky_danh_gias");
                entity.HasKey(e => e.IdKyDanhGia).HasName("ky_danh_gias_pkey");

                entity.Property(e => e.IdKyDanhGia).HasColumnName("id_ky_danh_gia").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.TenKyDanhGia).IsRequired().HasMaxLength(200).HasColumnName("ten_ky_danh_gia");
                entity.Property(e => e.Nam).HasColumnName("nam");
                entity.Property(e => e.NgayBatDau).HasColumnName("ngay_bat_dau");
                entity.Property(e => e.NgayKetThuc).HasColumnName("ngay_ket_thuc");
                entity.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50).HasColumnName("trang_thai");
            });

            modelBuilder.Entity<PhieuDanhGiaNangLuc>(entity =>
            {
                entity.ToTable("phieu_danh_gia_nang_lucs");
                entity.HasKey(e => e.IdPhieu).HasName("phieu_danh_gia_nang_lucs_pkey");

                entity.Property(e => e.IdPhieu).HasColumnName("id_phieu").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.IdKyDanhGia).HasColumnName("id_ky_danh_gia");
                entity.Property(e => e.CccdNhanVien).IsRequired().HasMaxLength(20).HasColumnName("cccd_nhan_vien");
                entity.Property(e => e.CccdQuanLy).HasMaxLength(20).HasColumnName("cccd_quan_ly");
                entity.Property(e => e.DiemTongHop).HasPrecision(5, 2).HasColumnName("diem_tong_hop");
                entity.Property(e => e.HeSoP2).HasPrecision(5, 2).HasColumnName("he_so_p2");
                entity.Property(e => e.XepLoai).HasMaxLength(100).HasColumnName("xep_loai");
                entity.Property(e => e.NhanXetChung).HasMaxLength(1000).HasColumnName("nhan_xet_chung");
                entity.Property(e => e.TrangThai).HasConversion<string>().HasMaxLength(50).HasColumnName("trang_thai");

                entity.HasOne(d => d.KyDanhGia)
                      .WithMany(p => p.PhieuDanhGias)
                      .HasForeignKey(d => d.IdKyDanhGia)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("phieu_danh_gias_id_ky_danh_gia_fkey");

                entity.HasOne(d => d.NhanVien)
                      .WithMany()
                      .HasForeignKey(d => d.CccdNhanVien)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("phieu_danh_gias_cccd_nhan_vien_fkey");

                entity.HasOne(d => d.QuanLy)
                      .WithMany()
                      .HasForeignKey(d => d.CccdQuanLy)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("phieu_danh_gias_cccd_quan_ly_fkey");
            });

            modelBuilder.Entity<ChiTietDanhGiaNangLuc>(entity =>
            {
                entity.ToTable("chi_tiet_danh_gia_nang_lucs");
                entity.HasKey(e => e.IdChiTiet).HasName("chi_tiet_danh_gia_nang_lucs_pkey");

                entity.Property(e => e.IdChiTiet).HasColumnName("id_chi_tiet").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.IdPhieu).HasColumnName("id_phieu");
                entity.Property(e => e.IdTieuChi).HasColumnName("id_tieu_chi");
                entity.Property(e => e.DiemTuDanhGia).HasColumnName("diem_tu_danh_gia");
                entity.Property(e => e.DiemQuanLyDanhGia).HasColumnName("diem_quan_ly_danh_gia");
                entity.Property(e => e.NhanXetNhanVien).HasMaxLength(500).HasColumnName("nhan_xet_nhan_vien");
                entity.Property(e => e.NhanXetQuanLy).HasMaxLength(500).HasColumnName("nhan_xet_quan_ly");

                entity.HasOne(d => d.PhieuDanhGia)
                      .WithMany(p => p.ChiTietDanhGias)
                      .HasForeignKey(d => d.IdPhieu)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("chi_tiet_danh_gias_id_phieu_fkey");

                entity.HasOne(d => d.TieuChi)
                      .WithMany()
                      .HasForeignKey(d => d.IdTieuChi)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("chi_tiet_danh_gias_id_tieu_chi_fkey");
            });

            modelBuilder.Entity<MucQuyDoiP2>(entity =>
            {
                entity.ToTable("muc_quy_doi_p2s");
                entity.HasKey(e => e.IdQuyDoi).HasName("muc_quy_doi_p2s_pkey");

                entity.Property(e => e.IdQuyDoi).HasColumnName("id_quy_doi").HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.XepLoai).IsRequired().HasMaxLength(100).HasColumnName("xep_loai");
                entity.Property(e => e.DiemToiThieu).HasPrecision(5, 2).HasColumnName("diem_toi_thieu");
                entity.Property(e => e.DiemToiDa).HasPrecision(5, 2).HasColumnName("diem_toi_da");
                entity.Property(e => e.HeSoP2).HasPrecision(5, 2).HasColumnName("he_so_p2");
            });



            ConfigureLichLamViec(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        private void ConfigureLichLamViec(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LichLamViec>(entity =>
            {
                entity.HasKey(e => e.IdLich).HasName("lich_lam_viecs_pkey");
                entity.ToTable("lich_lam_viecs");

                entity.Property(e => e.IdLich).HasColumnName("id_lich");
                entity.Property(e => e.Nam).HasColumnName("nam");
                entity.HasIndex(e => e.Nam).IsUnique().HasDatabaseName("idx_lich_lam_viec_nam_unique");

                entity.Property(e => e.TrangThai)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .HasColumnName("trang_thai");

                entity.Property(e => e.GhiChu)
                    .HasMaxLength(500)
                    .HasColumnName("ghi_chu");
            });

            modelBuilder.Entity<ChiTietLichLamViec>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("chi_tiet_lich_lam_viecs_pkey");
                entity.ToTable("chi_tiet_lich_lam_viecs");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdLich).HasColumnName("id_lich");
                entity.Property(e => e.Ngay).HasColumnName("ngay");

                entity.Property(e => e.Thu)
                    .HasMaxLength(20)
                    .HasColumnName("thu");

                entity.Property(e => e.LoaiNgay)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .HasColumnName("loai_ngay");

                entity.Property(e => e.TenNgayNghi)
                    .HasMaxLength(100)
                    .HasColumnName("ten_ngay_nghi");

                entity.Property(e => e.SoGioLam)
                    .HasColumnType("decimal(4,1)")
                    .HasColumnName("so_gio_lam");

                entity.HasOne(e => e.LichLamViec)
                    .WithMany(l => l.ChiTietLichLamViecs)
                    .HasForeignKey(e => e.IdLich)
                    .HasConstraintName("fk_chi_tiet_lich_lam_viec");

                entity.HasIndex(e => new { e.IdLich, e.Ngay })
                    .HasDatabaseName("idx_chi_tiet_lich_ngay");
            });
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
