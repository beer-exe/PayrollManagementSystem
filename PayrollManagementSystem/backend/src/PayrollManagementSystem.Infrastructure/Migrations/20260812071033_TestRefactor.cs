using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "bac_luongs_id_ngach_luong_fkey",
                table: "bac_luongs");

            migrationBuilder.DropForeignKey(
                name: "bang_luongs_cccd_nhan_vien_fkey",
                table: "bang_luongs");

            migrationBuilder.DropForeignKey(
                name: "bang_luongs_id_ky_luong_fkey",
                table: "bang_luongs");

            migrationBuilder.DropForeignKey(
                name: "cham_congs_cccd_nhan_vien_fkey",
                table: "cham_congs");

            migrationBuilder.DropForeignKey(
                name: "chi_tiet_danh_gias_id_phieu_fkey",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "chi_tiet_danh_gias_id_tieu_chi_fkey",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_lich_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_lich_lam_viec",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropForeignKey(
                name: "chuc_vus_id_chuc_vu_quan_ly_fkey",
                table: "chuc_vus");

            migrationBuilder.DropForeignKey(
                name: "chuc_vus_id_ngach_luong_fkey",
                table: "chuc_vus");

            migrationBuilder.DropForeignKey(
                name: "chuc_vus_id_phong_ban_fkey",
                table: "chuc_vus");

            migrationBuilder.DropForeignKey(
                name: "don_nghis_cccd_nguoi_duyet_fkey",
                table: "don_nghis");

            migrationBuilder.DropForeignKey(
                name: "don_nghis_cccd_nhan_vien_fkey",
                table: "don_nghis");

            migrationBuilder.DropForeignKey(
                name: "hop_dong_lao_dongs_cccd_fkey",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropForeignKey(
                name: "khung_gio_nghis_id_ca_lam_viec_fkey",
                table: "khung_gio_nghis");

            migrationBuilder.DropForeignKey(
                name: "khung_nang_luc_id_chuc_vu_fkey",
                table: "khung_nang_luc_p2");

            migrationBuilder.DropForeignKey(
                name: "ngay_phep_nhan_viens_cccd_fkey",
                table: "ngay_phep_nhan_viens");

            migrationBuilder.DropForeignKey(
                name: "nhan_viens_id_pb_fkey",
                table: "nhan_viens");

            migrationBuilder.DropForeignKey(
                name: "nhan_viens_id_tai_khoan_fkey",
                table: "nhan_viens");

            migrationBuilder.DropForeignKey(
                name: "nhat_ky_trang_thais_cccd_fkey",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropForeignKey(
                name: "fk_phan_cong_ca_ca_lam_viec",
                table: "phan_cong_cas");

            migrationBuilder.DropForeignKey(
                name: "fk_phan_cong_ca_nhan_vien",
                table: "phan_cong_cas");

            migrationBuilder.DropForeignKey(
                name: "phieu_danh_gias_cccd_nhan_vien_fkey",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "phieu_danh_gias_cccd_quan_ly_fkey",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "phieu_danh_gias_id_ky_danh_gia_fkey",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "quyet_dinh_nhan_sus_cccd_fkey",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropForeignKey(
                name: "quyet_dinh_nhan_sus_id_bac_luong_moi_fkey",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropForeignKey(
                name: "quyet_dinh_nhan_sus_id_chuc_vu_moi_fkey",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropForeignKey(
                name: "tai_khoans_id_vai_tro_fkey",
                table: "tai_khoans");

            migrationBuilder.DropForeignKey(
                name: "than_nhan_nhan_vien_cccd_fkey",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropForeignKey(
                name: "than_nhan_nhan_vien_id_mqh_fkey",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropForeignKey(
                name: "than_nhan_nhan_vien_ma_dinh_danh_fkey",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropPrimaryKey(
                name: "vai_tros_pkey",
                table: "vai_tros");

            migrationBuilder.DropPrimaryKey(
                name: "than_nhans_pkey",
                table: "than_nhans");

            migrationBuilder.DropPrimaryKey(
                name: "tai_khoans_pkey",
                table: "tai_khoans");

            migrationBuilder.DropPrimaryKey(
                name: "quyet_dinh_nhan_sus_pkey",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropPrimaryKey(
                name: "phong_bans_pkey",
                table: "phong_bans");

            migrationBuilder.DropPrimaryKey(
                name: "phieu_danh_gia_nang_lucs_pkey",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropPrimaryKey(
                name: "phan_cong_cas_pkey",
                table: "phan_cong_cas");

            migrationBuilder.DropPrimaryKey(
                name: "nhat_ky_trang_thais_pkey",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropPrimaryKey(
                name: "nhan_viens_pkey",
                table: "nhan_viens");

            migrationBuilder.DropPrimaryKey(
                name: "ngay_phep_nhan_viens_pkey",
                table: "ngay_phep_nhan_viens");

            migrationBuilder.DropPrimaryKey(
                name: "ngach_luongs_pkey",
                table: "ngach_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "muc_quy_doi_p2s_pkey",
                table: "muc_quy_doi_p2s");

            migrationBuilder.DropPrimaryKey(
                name: "moi_quan_hes_pkey",
                table: "moi_quan_hes");

            migrationBuilder.DropPrimaryKey(
                name: "lich_lam_viecs_pkey",
                table: "lich_lam_viecs");

            migrationBuilder.DropIndex(
                name: "idx_lich_lam_viec_nam_unique",
                table: "lich_lam_viecs");

            migrationBuilder.DropPrimaryKey(
                name: "ky_luongs_pkey",
                table: "ky_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "ky_danh_gias_pkey",
                table: "ky_danh_gias");

            migrationBuilder.DropPrimaryKey(
                name: "khung_gio_nghis_pkey",
                table: "khung_gio_nghis");

            migrationBuilder.DropPrimaryKey(
                name: "khoan_khau_trus_pkey",
                table: "khoan_khau_trus");

            migrationBuilder.DropPrimaryKey(
                name: "hop_dong_lao_dongs_pkey",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropPrimaryKey(
                name: "don_nghis_pkey",
                table: "don_nghis");

            migrationBuilder.DropPrimaryKey(
                name: "chuc_vus_pkey",
                table: "chuc_vus");

            migrationBuilder.DropPrimaryKey(
                name: "chi_tiet_lich_lam_viecs_pkey",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropPrimaryKey(
                name: "chi_tiet_danh_gia_nang_lucs_pkey",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropPrimaryKey(
                name: "cham_congs_pkey",
                table: "cham_congs");

            migrationBuilder.DropPrimaryKey(
                name: "cau_hinh_giam_trus_pkey",
                table: "cau_hinh_giam_trus");

            migrationBuilder.DropPrimaryKey(
                name: "ca_lam_viecs_pkey",
                table: "ca_lam_viecs");

            migrationBuilder.DropPrimaryKey(
                name: "bang_luongs_pkey",
                table: "bang_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "bac_thues_pkey",
                table: "bac_thues");

            migrationBuilder.DropPrimaryKey(
                name: "bac_luongs_pkey",
                table: "bac_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "than_nhan_nhan_vien_pkey",
                table: "than_nhan_nhan_vien");

            migrationBuilder.DropPrimaryKey(
                name: "khung_nang_luc_p2_pkey",
                table: "khung_nang_luc_p2");

            migrationBuilder.RenameTable(
                name: "than_nhan_nhan_vien",
                newName: "t_nhan_nviens");

            migrationBuilder.RenameTable(
                name: "khung_nang_luc_p2",
                newName: "khung_nang_luc_p2s");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "vai_tros",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "vai_tros",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "vai_tros",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "vai_tros",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "vai_tros",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "than_nhans",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "than_nhans",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "than_nhans",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "than_nhans",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "than_nhans",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UserAvatar",
                table: "tai_khoans",
                newName: "user_avatar");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "tai_khoans",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "tai_khoans",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "tai_khoans",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "tai_khoans",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "tai_khoans",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_tai_khoans_id_vai_tro",
                table: "tai_khoans",
                newName: "ix_tai_khoans_id_vai_tro");

            migrationBuilder.RenameIndex(
                name: "tai_khoans_ten_tai_khoan_key",
                table: "tai_khoans",
                newName: "ix_tai_khoans_ten_tai_khoan");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "quyet_dinh_nhan_sus",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "quyet_dinh_nhan_sus",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "quyet_dinh_nhan_sus",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "quyet_dinh_nhan_sus",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "quyet_dinh_nhan_sus",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_quyet_dinh_nhan_sus_id_chuc_vu_moi",
                table: "quyet_dinh_nhan_sus",
                newName: "ix_quyet_dinh_nhan_sus_id_chuc_vu_moi");

            migrationBuilder.RenameIndex(
                name: "IX_quyet_dinh_nhan_sus_id_bac_luong_moi",
                table: "quyet_dinh_nhan_sus",
                newName: "ix_quyet_dinh_nhan_sus_id_bac_luong_moi");

            migrationBuilder.RenameIndex(
                name: "idx_qd_nhansu_active",
                table: "quyet_dinh_nhan_sus",
                newName: "ix_quyet_dinh_nhan_sus_cccd_trang_thai_ngay_hieu_luc");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "phong_bans",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "phong_bans",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "phong_bans",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "phong_bans",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "phong_bans",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "phieu_danh_gia_nang_lucs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "phieu_danh_gia_nang_lucs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "phieu_danh_gia_nang_lucs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "phieu_danh_gia_nang_lucs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "phieu_danh_gia_nang_lucs",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_phieu_danh_gia_nang_lucs_id_ky_danh_gia",
                table: "phieu_danh_gia_nang_lucs",
                newName: "ix_phieu_danh_gia_nang_lucs_id_ky_danh_gia");

            migrationBuilder.RenameIndex(
                name: "IX_phieu_danh_gia_nang_lucs_cccd_quan_ly",
                table: "phieu_danh_gia_nang_lucs",
                newName: "ix_phieu_danh_gia_nang_lucs_cccd_quan_ly");

            migrationBuilder.RenameIndex(
                name: "IX_phieu_danh_gia_nang_lucs_cccd_nhan_vien",
                table: "phieu_danh_gia_nang_lucs",
                newName: "ix_phieu_danh_gia_nang_lucs_cccd_nhan_vien");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "phan_cong_cas",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "phan_cong_cas",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "phan_cong_cas",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "phan_cong_cas",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "phan_cong_cas",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_phan_cong_cas_id_ca_lam_viec",
                table: "phan_cong_cas",
                newName: "ix_phan_cong_cas_id_ca_lam_viec");

            migrationBuilder.RenameIndex(
                name: "idx_phan_cong_ca_nv_ngay_unique",
                table: "phan_cong_cas",
                newName: "ix_phan_cong_cas_cccd_nhan_vien_ngay_lam_viec");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "nhat_ky_trang_thais",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "nhat_ky_trang_thais",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "nhat_ky_trang_thais",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "nhat_ky_trang_thais",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "nhat_ky_trang_thais",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_nhat_ky_trang_thais_cccd",
                table: "nhat_ky_trang_thais",
                newName: "ix_nhat_ky_trang_thais_cccd");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "nhan_viens",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "nhan_viens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "nhan_viens",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IdTaiKhoan",
                table: "nhan_viens",
                newName: "id_tai_khoan");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "nhan_viens",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "nhan_viens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_nhan_viens_id_pb",
                table: "nhan_viens",
                newName: "ix_nhan_viens_id_pb");

            migrationBuilder.RenameIndex(
                name: "nhan_viens_email_key",
                table: "nhan_viens",
                newName: "ix_nhan_viens_email");

            migrationBuilder.RenameIndex(
                name: "IX_nhan_viens_IdTaiKhoan",
                table: "nhan_viens",
                newName: "ix_nhan_viens_id_tai_khoan");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ngay_phep_nhan_viens",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ngay_phep_nhan_viens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ngay_phep_nhan_viens",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ngay_phep_nhan_viens",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ngay_phep_nhan_viens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "ngay_phep_nhan_viens_cccd_nam_unique",
                table: "ngay_phep_nhan_viens",
                newName: "ix_ngay_phep_nhan_viens_cccd_nhan_vien_nam");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ngach_luongs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ngach_luongs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "ngach_luongs",
                newName: "trang_thai");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ngach_luongs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ngach_luongs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ngach_luongs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "muc_quy_doi_p2s",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "muc_quy_doi_p2s",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "muc_quy_doi_p2s",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "muc_quy_doi_p2s",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "muc_quy_doi_p2s",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "moi_quan_hes",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "moi_quan_hes",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "moi_quan_hes",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "moi_quan_hes",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "moi_quan_hes",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "lich_lam_viecs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "lich_lam_viecs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "lich_lam_viecs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "lich_lam_viecs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "lich_lam_viecs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ky_luongs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ky_luongs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ky_luongs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ky_luongs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ky_luongs",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "idx_ky_luong_thang_nam",
                table: "ky_luongs",
                newName: "ix_ky_luongs_thang_nam");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ky_danh_gias",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ky_danh_gias",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ky_danh_gias",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ky_danh_gias",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ky_danh_gias",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "khung_gio_nghis",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "khung_gio_nghis",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "khung_gio_nghis",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "khung_gio_nghis",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "khung_gio_nghis",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_khung_gio_nghis_id_ca_lam_viec",
                table: "khung_gio_nghis",
                newName: "ix_khung_gio_nghis_id_ca_lam_viec");

            migrationBuilder.RenameIndex(
                name: "idx_khoan_khau_tru_ten",
                table: "khoan_khau_trus",
                newName: "ix_khoan_khau_trus_ten_khoan_khau_tru");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "hop_dong_lao_dongs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "hop_dong_lao_dongs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "hop_dong_lao_dongs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "hop_dong_lao_dongs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "hop_dong_lao_dongs",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_hop_dong_lao_dongs_cccd",
                table: "hop_dong_lao_dongs",
                newName: "ix_hop_dong_lao_dongs_cccd");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "don_nghis",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "don_nghis",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "don_nghis",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "don_nghis",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "don_nghis",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_don_nghis_cccd_nguoi_duyet",
                table: "don_nghis",
                newName: "ix_don_nghis_cccd_nguoi_duyet");

            migrationBuilder.RenameIndex(
                name: "idx_don_nghis_nv_ngay",
                table: "don_nghis",
                newName: "ix_don_nghis_cccd_nhan_vien_ngay_bat_dau_ngay_ket_thuc");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "chuc_vus",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "chuc_vus",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "chuc_vus",
                newName: "trang_thai");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "chuc_vus",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "chuc_vus",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "chuc_vus",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_chuc_vus_id_phong_ban",
                table: "chuc_vus",
                newName: "ix_chuc_vus_id_phong_ban");

            migrationBuilder.RenameIndex(
                name: "IX_chuc_vus_id_ngach_luong",
                table: "chuc_vus",
                newName: "ix_chuc_vus_id_ngach_luong");

            migrationBuilder.RenameIndex(
                name: "IX_chuc_vus_id_chuc_vu_quan_ly",
                table: "chuc_vus",
                newName: "ix_chuc_vus_id_chuc_vu_quan_ly");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "chi_tiet_lich_lam_viecs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "chi_tiet_lich_lam_viecs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "chi_tiet_lich_lam_viecs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "chi_tiet_lich_lam_viecs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "chi_tiet_lich_lam_viecs",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_chi_tiet_lich_lam_viecs_id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs",
                newName: "ix_chi_tiet_lich_lam_viecs_id_ca_lam_viec_mac_dinh");

            migrationBuilder.RenameIndex(
                name: "idx_chi_tiet_lich_ngay",
                table: "chi_tiet_lich_lam_viecs",
                newName: "ix_chi_tiet_lich_lam_viecs_id_lich_ngay");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_chi_tiet_danh_gia_nang_lucs_id_tieu_chi",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "ix_chi_tiet_danh_gia_nang_lucs_id_tieu_chi");

            migrationBuilder.RenameIndex(
                name: "IX_chi_tiet_danh_gia_nang_lucs_id_phieu",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "ix_chi_tiet_danh_gia_nang_lucs_id_phieu");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "cham_congs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "cham_congs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SoPhutVeSom",
                table: "cham_congs",
                newName: "so_phut_ve_som");

            migrationBuilder.RenameColumn(
                name: "SoPhutDiTre",
                table: "cham_congs",
                newName: "so_phut_di_tre");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "cham_congs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "cham_congs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "cham_congs",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "cham_congs_cccd_ngay_unique",
                table: "cham_congs",
                newName: "ix_cham_congs_cccd_nhan_vien_ngay_cham_cong");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "ca_lam_viecs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ca_lam_viecs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ca_lam_viecs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ca_lam_viecs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ca_lam_viecs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "bang_luongs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "bang_luongs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "bang_luongs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "GioCongThucTe",
                table: "bang_luongs",
                newName: "gio_cong_thuc_te");

            migrationBuilder.RenameColumn(
                name: "GioCongChuan",
                table: "bang_luongs",
                newName: "gio_cong_chuan");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "bang_luongs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "bang_luongs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ChiTietThue",
                table: "bang_luongs",
                newName: "chi_tiet_thue");

            migrationBuilder.RenameColumn(
                name: "ChiTietKhauTru",
                table: "bang_luongs",
                newName: "chi_tiet_khau_tru");

            migrationBuilder.RenameIndex(
                name: "IX_bang_luongs_cccd_nhan_vien",
                table: "bang_luongs",
                newName: "ix_bang_luongs_cccd_nhan_vien");

            migrationBuilder.RenameIndex(
                name: "idx_bang_luong_ky_nv",
                table: "bang_luongs",
                newName: "ix_bang_luongs_id_ky_luong_cccd_nhan_vien");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "bac_luongs",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "bac_luongs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "bac_luongs",
                newName: "trang_thai");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "bac_luongs",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "bac_luongs",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "bac_luongs",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_bac_luongs_id_ngach_luong",
                table: "bac_luongs",
                newName: "ix_bac_luongs_id_ngach_luong");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "t_nhan_nviens",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "t_nhan_nviens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "LaNguoiPhuThuoc",
                table: "t_nhan_nviens",
                newName: "la_nguoi_phu_thuoc");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "t_nhan_nviens",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "t_nhan_nviens",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "t_nhan_nviens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_than_nhan_nhan_vien_ma_dinh_danh",
                table: "t_nhan_nviens",
                newName: "ix_t_nhan_nviens_ma_dinh_danh");

            migrationBuilder.RenameIndex(
                name: "IX_than_nhan_nhan_vien_id_mqh",
                table: "t_nhan_nviens",
                newName: "ix_t_nhan_nviens_id_mqh");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "khung_nang_luc_p2s",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "khung_nang_luc_p2s",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "khung_nang_luc_p2s",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "khung_nang_luc_p2s",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "khung_nang_luc_p2s",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_khung_nang_luc_p2_id_chuc_vu",
                table: "khung_nang_luc_p2s",
                newName: "ix_khung_nang_luc_p2s_id_chuc_vu");

            migrationBuilder.AddPrimaryKey(
                name: "pk_vai_tros",
                table: "vai_tros",
                column: "id_vai_tro");

            migrationBuilder.AddPrimaryKey(
                name: "pk_than_nhans",
                table: "than_nhans",
                column: "ma_dinh_danh");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tai_khoans",
                table: "tai_khoans",
                column: "id_tai_khoan");

            migrationBuilder.AddPrimaryKey(
                name: "pk_quyet_dinh_nhan_sus",
                table: "quyet_dinh_nhan_sus",
                column: "so_quyet_dinh");

            migrationBuilder.AddPrimaryKey(
                name: "pk_phong_bans",
                table: "phong_bans",
                column: "id_pb");

            migrationBuilder.AddPrimaryKey(
                name: "pk_phieu_danh_gia_nang_lucs",
                table: "phieu_danh_gia_nang_lucs",
                column: "id_phieu");

            migrationBuilder.AddPrimaryKey(
                name: "pk_phan_cong_cas",
                table: "phan_cong_cas",
                column: "id_phan_cong");

            migrationBuilder.AddPrimaryKey(
                name: "pk_nhat_ky_trang_thais",
                table: "nhat_ky_trang_thais",
                column: "id_nhat_ky");

            migrationBuilder.AddPrimaryKey(
                name: "ak_nhan_viens_cccd",
                table: "nhan_viens",
                column: "cccd");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ngay_phep_nhan_viens",
                table: "ngay_phep_nhan_viens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ngach_luongs",
                table: "ngach_luongs",
                column: "id_ngach_luong");

            migrationBuilder.AddPrimaryKey(
                name: "pk_muc_quy_doi_p2s",
                table: "muc_quy_doi_p2s",
                column: "id_quy_doi");

            migrationBuilder.AddPrimaryKey(
                name: "pk_moi_quan_hes",
                table: "moi_quan_hes",
                column: "id_mqh");

            migrationBuilder.AddPrimaryKey(
                name: "pk_lich_lam_viecs",
                table: "lich_lam_viecs",
                column: "id_lich");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ky_luongs",
                table: "ky_luongs",
                column: "id_ky_luong");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ky_danh_gias",
                table: "ky_danh_gias",
                column: "id_ky_danh_gia");

            migrationBuilder.AddPrimaryKey(
                name: "pk_khung_gio_nghis",
                table: "khung_gio_nghis",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_khoan_khau_trus",
                table: "khoan_khau_trus",
                column: "id_khoan_khau_tru");

            migrationBuilder.AddPrimaryKey(
                name: "pk_hop_dong_lao_dongs",
                table: "hop_dong_lao_dongs",
                column: "so_hop_dong");

            migrationBuilder.AddPrimaryKey(
                name: "pk_don_nghis",
                table: "don_nghis",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chuc_vus",
                table: "chuc_vus",
                column: "id_chuc_vu");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chi_tiet_lich_lam_viecs",
                table: "chi_tiet_lich_lam_viecs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_chi_tiet_danh_gia_nang_lucs",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_chi_tiet");

            migrationBuilder.AddPrimaryKey(
                name: "pk_cham_congs",
                table: "cham_congs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_cau_hinh_giam_trus",
                table: "cau_hinh_giam_trus",
                column: "id_cau_hinh_giam_tru");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ca_lam_viecs",
                table: "ca_lam_viecs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bang_luongs",
                table: "bang_luongs",
                column: "id_bang_luong");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bac_thues",
                table: "bac_thues",
                column: "id_bac_thue");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bac_luongs",
                table: "bac_luongs",
                column: "id_bac_luong");

            migrationBuilder.AddPrimaryKey(
                name: "pk_t_nhan_nviens",
                table: "t_nhan_nviens",
                columns: new[] { "cccd", "ma_dinh_danh" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_khung_nang_luc_p2s",
                table: "khung_nang_luc_p2s",
                column: "id_tieu_chi");

            migrationBuilder.CreateIndex(
                name: "ix_lich_lam_viecs_nam",
                table: "lich_lam_viecs",
                column: "nam",
                unique: true,
                filter: "\"is_deleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "fk_bac_luongs_ngach_luongs_id_ngach_luong",
                table: "bac_luongs",
                column: "id_ngach_luong",
                principalTable: "ngach_luongs",
                principalColumn: "id_ngach_luong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bang_luongs_ky_luongs_id_ky_luong",
                table: "bang_luongs",
                column: "id_ky_luong",
                principalTable: "ky_luongs",
                principalColumn: "id_ky_luong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_bang_luongs_nhan_viens_cccd_nhan_vien",
                table: "bang_luongs",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cham_congs_nhan_viens_cccd_nhan_vien",
                table: "cham_congs",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_danh_gia_nang_lucs_khung_nang_luc_p2s_id_tieu_chi",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_tieu_chi",
                principalTable: "khung_nang_luc_p2s",
                principalColumn: "id_tieu_chi",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_danh_gia_nang_lucs_phieu_danh_gia_nang_lucs_id_phi",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_phieu",
                principalTable: "phieu_danh_gia_nang_lucs",
                principalColumn: "id_phieu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_lich_lam_viecs_ca_lam_viecs_id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs",
                column: "id_ca_lam_viec_mac_dinh",
                principalTable: "ca_lam_viecs",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_lich_lam_viecs_lich_lam_viecs_id_lich",
                table: "chi_tiet_lich_lam_viecs",
                column: "id_lich",
                principalTable: "lich_lam_viecs",
                principalColumn: "id_lich",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chuc_vus_chuc_vus_id_chuc_vu_quan_ly",
                table: "chuc_vus",
                column: "id_chuc_vu_quan_ly",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_chuc_vus_ngach_luongs_id_ngach_luong",
                table: "chuc_vus",
                column: "id_ngach_luong",
                principalTable: "ngach_luongs",
                principalColumn: "id_ngach_luong",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_chuc_vus_phong_bans_id_phong_ban",
                table: "chuc_vus",
                column: "id_phong_ban",
                principalTable: "phong_bans",
                principalColumn: "id_pb",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_don_nghis_nhan_viens_cccd_nguoi_duyet",
                table: "don_nghis",
                column: "cccd_nguoi_duyet",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_don_nghis_nhan_viens_cccd_nhan_vien",
                table: "don_nghis",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_hop_dong_lao_dongs_nhan_viens_cccd",
                table: "hop_dong_lao_dongs",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_khung_gio_nghis_ca_lam_viecs_id_ca_lam_viec",
                table: "khung_gio_nghis",
                column: "id_ca_lam_viec",
                principalTable: "ca_lam_viecs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_khung_nang_luc_p2s_chuc_vus_id_chuc_vu",
                table: "khung_nang_luc_p2s",
                column: "id_chuc_vu",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ngay_phep_nhan_viens_nhan_viens_cccd_nhan_vien",
                table: "ngay_phep_nhan_viens",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_nhan_viens_phong_bans_id_pb",
                table: "nhan_viens",
                column: "id_pb",
                principalTable: "phong_bans",
                principalColumn: "id_pb",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_nhan_viens_tai_khoans_id_tai_khoan",
                table: "nhan_viens",
                column: "id_tai_khoan",
                principalTable: "tai_khoans",
                principalColumn: "id_tai_khoan",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_nhat_ky_trang_thais_nhan_viens_cccd",
                table: "nhat_ky_trang_thais",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_phan_cong_cas_ca_lam_viecs_id_ca_lam_viec",
                table: "phan_cong_cas",
                column: "id_ca_lam_viec",
                principalTable: "ca_lam_viecs",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_phan_cong_cas_nhan_viens_cccd_nhan_vien",
                table: "phan_cong_cas",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_danh_gia_nang_lucs_ky_danh_gias_id_ky_danh_gia",
                table: "phieu_danh_gia_nang_lucs",
                column: "id_ky_danh_gia",
                principalTable: "ky_danh_gias",
                principalColumn: "id_ky_danh_gia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_danh_gia_nang_lucs_nhan_viens_cccd_nhan_vien",
                table: "phieu_danh_gia_nang_lucs",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_phieu_danh_gia_nang_lucs_nhan_viens_cccd_quan_ly",
                table: "phieu_danh_gia_nang_lucs",
                column: "cccd_quan_ly",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_quyet_dinh_nhan_sus_bac_luongs_id_bac_luong_moi",
                table: "quyet_dinh_nhan_sus",
                column: "id_bac_luong_moi",
                principalTable: "bac_luongs",
                principalColumn: "id_bac_luong",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_quyet_dinh_nhan_sus_chuc_vus_id_chuc_vu_moi",
                table: "quyet_dinh_nhan_sus",
                column: "id_chuc_vu_moi",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_quyet_dinh_nhan_sus_nhan_viens_cccd",
                table: "quyet_dinh_nhan_sus",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_t_nhan_nviens_moi_quan_hes_id_mqh",
                table: "t_nhan_nviens",
                column: "id_mqh",
                principalTable: "moi_quan_hes",
                principalColumn: "id_mqh",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_t_nhan_nviens_nhan_viens_cccd",
                table: "t_nhan_nviens",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_t_nhan_nviens_than_nhans_ma_dinh_danh",
                table: "t_nhan_nviens",
                column: "ma_dinh_danh",
                principalTable: "than_nhans",
                principalColumn: "ma_dinh_danh",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tai_khoans_vai_tros_id_vai_tro",
                table: "tai_khoans",
                column: "id_vai_tro",
                principalTable: "vai_tros",
                principalColumn: "id_vai_tro",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_bac_luongs_ngach_luongs_id_ngach_luong",
                table: "bac_luongs");

            migrationBuilder.DropForeignKey(
                name: "fk_bang_luongs_ky_luongs_id_ky_luong",
                table: "bang_luongs");

            migrationBuilder.DropForeignKey(
                name: "fk_bang_luongs_nhan_viens_cccd_nhan_vien",
                table: "bang_luongs");

            migrationBuilder.DropForeignKey(
                name: "fk_cham_congs_nhan_viens_cccd_nhan_vien",
                table: "cham_congs");

            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_danh_gia_nang_lucs_khung_nang_luc_p2s_id_tieu_chi",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_danh_gia_nang_lucs_phieu_danh_gia_nang_lucs_id_phi",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_lich_lam_viecs_ca_lam_viecs_id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropForeignKey(
                name: "fk_chi_tiet_lich_lam_viecs_lich_lam_viecs_id_lich",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropForeignKey(
                name: "fk_chuc_vus_chuc_vus_id_chuc_vu_quan_ly",
                table: "chuc_vus");

            migrationBuilder.DropForeignKey(
                name: "fk_chuc_vus_ngach_luongs_id_ngach_luong",
                table: "chuc_vus");

            migrationBuilder.DropForeignKey(
                name: "fk_chuc_vus_phong_bans_id_phong_ban",
                table: "chuc_vus");

            migrationBuilder.DropForeignKey(
                name: "fk_don_nghis_nhan_viens_cccd_nguoi_duyet",
                table: "don_nghis");

            migrationBuilder.DropForeignKey(
                name: "fk_don_nghis_nhan_viens_cccd_nhan_vien",
                table: "don_nghis");

            migrationBuilder.DropForeignKey(
                name: "fk_hop_dong_lao_dongs_nhan_viens_cccd",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropForeignKey(
                name: "fk_khung_gio_nghis_ca_lam_viecs_id_ca_lam_viec",
                table: "khung_gio_nghis");

            migrationBuilder.DropForeignKey(
                name: "fk_khung_nang_luc_p2s_chuc_vus_id_chuc_vu",
                table: "khung_nang_luc_p2s");

            migrationBuilder.DropForeignKey(
                name: "fk_ngay_phep_nhan_viens_nhan_viens_cccd_nhan_vien",
                table: "ngay_phep_nhan_viens");

            migrationBuilder.DropForeignKey(
                name: "fk_nhan_viens_phong_bans_id_pb",
                table: "nhan_viens");

            migrationBuilder.DropForeignKey(
                name: "fk_nhan_viens_tai_khoans_id_tai_khoan",
                table: "nhan_viens");

            migrationBuilder.DropForeignKey(
                name: "fk_nhat_ky_trang_thais_nhan_viens_cccd",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropForeignKey(
                name: "fk_phan_cong_cas_ca_lam_viecs_id_ca_lam_viec",
                table: "phan_cong_cas");

            migrationBuilder.DropForeignKey(
                name: "fk_phan_cong_cas_nhan_viens_cccd_nhan_vien",
                table: "phan_cong_cas");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_danh_gia_nang_lucs_ky_danh_gias_id_ky_danh_gia",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_danh_gia_nang_lucs_nhan_viens_cccd_nhan_vien",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "fk_phieu_danh_gia_nang_lucs_nhan_viens_cccd_quan_ly",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropForeignKey(
                name: "fk_quyet_dinh_nhan_sus_bac_luongs_id_bac_luong_moi",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropForeignKey(
                name: "fk_quyet_dinh_nhan_sus_chuc_vus_id_chuc_vu_moi",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropForeignKey(
                name: "fk_quyet_dinh_nhan_sus_nhan_viens_cccd",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropForeignKey(
                name: "fk_t_nhan_nviens_moi_quan_hes_id_mqh",
                table: "t_nhan_nviens");

            migrationBuilder.DropForeignKey(
                name: "fk_t_nhan_nviens_nhan_viens_cccd",
                table: "t_nhan_nviens");

            migrationBuilder.DropForeignKey(
                name: "fk_t_nhan_nviens_than_nhans_ma_dinh_danh",
                table: "t_nhan_nviens");

            migrationBuilder.DropForeignKey(
                name: "fk_tai_khoans_vai_tros_id_vai_tro",
                table: "tai_khoans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_vai_tros",
                table: "vai_tros");

            migrationBuilder.DropPrimaryKey(
                name: "pk_than_nhans",
                table: "than_nhans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tai_khoans",
                table: "tai_khoans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_quyet_dinh_nhan_sus",
                table: "quyet_dinh_nhan_sus");

            migrationBuilder.DropPrimaryKey(
                name: "pk_phong_bans",
                table: "phong_bans");

            migrationBuilder.DropPrimaryKey(
                name: "pk_phieu_danh_gia_nang_lucs",
                table: "phieu_danh_gia_nang_lucs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_phan_cong_cas",
                table: "phan_cong_cas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_nhat_ky_trang_thais",
                table: "nhat_ky_trang_thais");

            migrationBuilder.DropPrimaryKey(
                name: "ak_nhan_viens_cccd",
                table: "nhan_viens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ngay_phep_nhan_viens",
                table: "ngay_phep_nhan_viens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ngach_luongs",
                table: "ngach_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_muc_quy_doi_p2s",
                table: "muc_quy_doi_p2s");

            migrationBuilder.DropPrimaryKey(
                name: "pk_moi_quan_hes",
                table: "moi_quan_hes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_lich_lam_viecs",
                table: "lich_lam_viecs");

            migrationBuilder.DropIndex(
                name: "ix_lich_lam_viecs_nam",
                table: "lich_lam_viecs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ky_luongs",
                table: "ky_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ky_danh_gias",
                table: "ky_danh_gias");

            migrationBuilder.DropPrimaryKey(
                name: "pk_khung_gio_nghis",
                table: "khung_gio_nghis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_khoan_khau_trus",
                table: "khoan_khau_trus");

            migrationBuilder.DropPrimaryKey(
                name: "pk_hop_dong_lao_dongs",
                table: "hop_dong_lao_dongs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_don_nghis",
                table: "don_nghis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chuc_vus",
                table: "chuc_vus");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chi_tiet_lich_lam_viecs",
                table: "chi_tiet_lich_lam_viecs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_chi_tiet_danh_gia_nang_lucs",
                table: "chi_tiet_danh_gia_nang_lucs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_cham_congs",
                table: "cham_congs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_cau_hinh_giam_trus",
                table: "cau_hinh_giam_trus");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ca_lam_viecs",
                table: "ca_lam_viecs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bang_luongs",
                table: "bang_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bac_thues",
                table: "bac_thues");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bac_luongs",
                table: "bac_luongs");

            migrationBuilder.DropPrimaryKey(
                name: "pk_t_nhan_nviens",
                table: "t_nhan_nviens");

            migrationBuilder.DropPrimaryKey(
                name: "pk_khung_nang_luc_p2s",
                table: "khung_nang_luc_p2s");

            migrationBuilder.RenameTable(
                name: "t_nhan_nviens",
                newName: "than_nhan_nhan_vien");

            migrationBuilder.RenameTable(
                name: "khung_nang_luc_p2s",
                newName: "khung_nang_luc_p2");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "vai_tros",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "vai_tros",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "vai_tros",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "vai_tros",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "vai_tros",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "than_nhans",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "than_nhans",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "than_nhans",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "than_nhans",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "than_nhans",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "user_avatar",
                table: "tai_khoans",
                newName: "UserAvatar");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "tai_khoans",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "tai_khoans",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "tai_khoans",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "tai_khoans",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "tai_khoans",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_tai_khoans_id_vai_tro",
                table: "tai_khoans",
                newName: "IX_tai_khoans_id_vai_tro");

            migrationBuilder.RenameIndex(
                name: "ix_tai_khoans_ten_tai_khoan",
                table: "tai_khoans",
                newName: "tai_khoans_ten_tai_khoan_key");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "quyet_dinh_nhan_sus",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "quyet_dinh_nhan_sus",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "quyet_dinh_nhan_sus",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "quyet_dinh_nhan_sus",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "quyet_dinh_nhan_sus",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_quyet_dinh_nhan_sus_id_chuc_vu_moi",
                table: "quyet_dinh_nhan_sus",
                newName: "IX_quyet_dinh_nhan_sus_id_chuc_vu_moi");

            migrationBuilder.RenameIndex(
                name: "ix_quyet_dinh_nhan_sus_id_bac_luong_moi",
                table: "quyet_dinh_nhan_sus",
                newName: "IX_quyet_dinh_nhan_sus_id_bac_luong_moi");

            migrationBuilder.RenameIndex(
                name: "ix_quyet_dinh_nhan_sus_cccd_trang_thai_ngay_hieu_luc",
                table: "quyet_dinh_nhan_sus",
                newName: "idx_qd_nhansu_active");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "phong_bans",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "phong_bans",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "phong_bans",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "phong_bans",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "phong_bans",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "phieu_danh_gia_nang_lucs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "phieu_danh_gia_nang_lucs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "phieu_danh_gia_nang_lucs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "phieu_danh_gia_nang_lucs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "phieu_danh_gia_nang_lucs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_danh_gia_nang_lucs_id_ky_danh_gia",
                table: "phieu_danh_gia_nang_lucs",
                newName: "IX_phieu_danh_gia_nang_lucs_id_ky_danh_gia");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_danh_gia_nang_lucs_cccd_quan_ly",
                table: "phieu_danh_gia_nang_lucs",
                newName: "IX_phieu_danh_gia_nang_lucs_cccd_quan_ly");

            migrationBuilder.RenameIndex(
                name: "ix_phieu_danh_gia_nang_lucs_cccd_nhan_vien",
                table: "phieu_danh_gia_nang_lucs",
                newName: "IX_phieu_danh_gia_nang_lucs_cccd_nhan_vien");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "phan_cong_cas",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "phan_cong_cas",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "phan_cong_cas",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "phan_cong_cas",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "phan_cong_cas",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_phan_cong_cas_id_ca_lam_viec",
                table: "phan_cong_cas",
                newName: "IX_phan_cong_cas_id_ca_lam_viec");

            migrationBuilder.RenameIndex(
                name: "ix_phan_cong_cas_cccd_nhan_vien_ngay_lam_viec",
                table: "phan_cong_cas",
                newName: "idx_phan_cong_ca_nv_ngay_unique");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "nhat_ky_trang_thais",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "nhat_ky_trang_thais",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "nhat_ky_trang_thais",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "nhat_ky_trang_thais",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "nhat_ky_trang_thais",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_nhat_ky_trang_thais_cccd",
                table: "nhat_ky_trang_thais",
                newName: "IX_nhat_ky_trang_thais_cccd");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "nhan_viens",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "nhan_viens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "nhan_viens",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "id_tai_khoan",
                table: "nhan_viens",
                newName: "IdTaiKhoan");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "nhan_viens",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "nhan_viens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_nhan_viens_id_pb",
                table: "nhan_viens",
                newName: "IX_nhan_viens_id_pb");

            migrationBuilder.RenameIndex(
                name: "ix_nhan_viens_id_tai_khoan",
                table: "nhan_viens",
                newName: "IX_nhan_viens_IdTaiKhoan");

            migrationBuilder.RenameIndex(
                name: "ix_nhan_viens_email",
                table: "nhan_viens",
                newName: "nhan_viens_email_key");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "ngay_phep_nhan_viens",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ngay_phep_nhan_viens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ngay_phep_nhan_viens",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "ngay_phep_nhan_viens",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ngay_phep_nhan_viens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_ngay_phep_nhan_viens_cccd_nhan_vien_nam",
                table: "ngay_phep_nhan_viens",
                newName: "ngay_phep_nhan_viens_cccd_nam_unique");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "ngach_luongs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ngach_luongs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "trang_thai",
                table: "ngach_luongs",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ngach_luongs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "ngach_luongs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ngach_luongs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "muc_quy_doi_p2s",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "muc_quy_doi_p2s",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "muc_quy_doi_p2s",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "muc_quy_doi_p2s",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "muc_quy_doi_p2s",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "moi_quan_hes",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "moi_quan_hes",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "moi_quan_hes",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "moi_quan_hes",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "moi_quan_hes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "lich_lam_viecs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "lich_lam_viecs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "lich_lam_viecs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "lich_lam_viecs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "lich_lam_viecs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "ky_luongs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ky_luongs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ky_luongs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "ky_luongs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ky_luongs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_ky_luongs_thang_nam",
                table: "ky_luongs",
                newName: "idx_ky_luong_thang_nam");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "ky_danh_gias",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ky_danh_gias",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ky_danh_gias",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "ky_danh_gias",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ky_danh_gias",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "khung_gio_nghis",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "khung_gio_nghis",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "khung_gio_nghis",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "khung_gio_nghis",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "khung_gio_nghis",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_khung_gio_nghis_id_ca_lam_viec",
                table: "khung_gio_nghis",
                newName: "IX_khung_gio_nghis_id_ca_lam_viec");

            migrationBuilder.RenameIndex(
                name: "ix_khoan_khau_trus_ten_khoan_khau_tru",
                table: "khoan_khau_trus",
                newName: "idx_khoan_khau_tru_ten");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "hop_dong_lao_dongs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "hop_dong_lao_dongs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "hop_dong_lao_dongs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "hop_dong_lao_dongs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "hop_dong_lao_dongs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_hop_dong_lao_dongs_cccd",
                table: "hop_dong_lao_dongs",
                newName: "IX_hop_dong_lao_dongs_cccd");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "don_nghis",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "don_nghis",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "don_nghis",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "don_nghis",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "don_nghis",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_don_nghis_cccd_nguoi_duyet",
                table: "don_nghis",
                newName: "IX_don_nghis_cccd_nguoi_duyet");

            migrationBuilder.RenameIndex(
                name: "ix_don_nghis_cccd_nhan_vien_ngay_bat_dau_ngay_ket_thuc",
                table: "don_nghis",
                newName: "idx_don_nghis_nv_ngay");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "chuc_vus",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "chuc_vus",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "trang_thai",
                table: "chuc_vus",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "chuc_vus",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "chuc_vus",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "chuc_vus",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_chuc_vus_id_phong_ban",
                table: "chuc_vus",
                newName: "IX_chuc_vus_id_phong_ban");

            migrationBuilder.RenameIndex(
                name: "ix_chuc_vus_id_ngach_luong",
                table: "chuc_vus",
                newName: "IX_chuc_vus_id_ngach_luong");

            migrationBuilder.RenameIndex(
                name: "ix_chuc_vus_id_chuc_vu_quan_ly",
                table: "chuc_vus",
                newName: "IX_chuc_vus_id_chuc_vu_quan_ly");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "chi_tiet_lich_lam_viecs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "chi_tiet_lich_lam_viecs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "chi_tiet_lich_lam_viecs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "chi_tiet_lich_lam_viecs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "chi_tiet_lich_lam_viecs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_chi_tiet_lich_lam_viecs_id_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs",
                newName: "IX_chi_tiet_lich_lam_viecs_id_ca_lam_viec_mac_dinh");

            migrationBuilder.RenameIndex(
                name: "ix_chi_tiet_lich_lam_viecs_id_lich_ngay",
                table: "chi_tiet_lich_lam_viecs",
                newName: "idx_chi_tiet_lich_ngay");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_chi_tiet_danh_gia_nang_lucs_id_tieu_chi",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "IX_chi_tiet_danh_gia_nang_lucs_id_tieu_chi");

            migrationBuilder.RenameIndex(
                name: "ix_chi_tiet_danh_gia_nang_lucs_id_phieu",
                table: "chi_tiet_danh_gia_nang_lucs",
                newName: "IX_chi_tiet_danh_gia_nang_lucs_id_phieu");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "cham_congs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "cham_congs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "so_phut_ve_som",
                table: "cham_congs",
                newName: "SoPhutVeSom");

            migrationBuilder.RenameColumn(
                name: "so_phut_di_tre",
                table: "cham_congs",
                newName: "SoPhutDiTre");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "cham_congs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "cham_congs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "cham_congs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_cham_congs_cccd_nhan_vien_ngay_cham_cong",
                table: "cham_congs",
                newName: "cham_congs_cccd_ngay_unique");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "ca_lam_viecs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ca_lam_viecs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ca_lam_viecs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "ca_lam_viecs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ca_lam_viecs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "bang_luongs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "bang_luongs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "bang_luongs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "gio_cong_thuc_te",
                table: "bang_luongs",
                newName: "GioCongThucTe");

            migrationBuilder.RenameColumn(
                name: "gio_cong_chuan",
                table: "bang_luongs",
                newName: "GioCongChuan");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "bang_luongs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "bang_luongs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "chi_tiet_thue",
                table: "bang_luongs",
                newName: "ChiTietThue");

            migrationBuilder.RenameColumn(
                name: "chi_tiet_khau_tru",
                table: "bang_luongs",
                newName: "ChiTietKhauTru");

            migrationBuilder.RenameIndex(
                name: "ix_bang_luongs_cccd_nhan_vien",
                table: "bang_luongs",
                newName: "IX_bang_luongs_cccd_nhan_vien");

            migrationBuilder.RenameIndex(
                name: "ix_bang_luongs_id_ky_luong_cccd_nhan_vien",
                table: "bang_luongs",
                newName: "idx_bang_luong_ky_nv");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "bac_luongs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "bac_luongs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "trang_thai",
                table: "bac_luongs",
                newName: "TrangThai");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "bac_luongs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "bac_luongs",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "bac_luongs",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_bac_luongs_id_ngach_luong",
                table: "bac_luongs",
                newName: "IX_bac_luongs_id_ngach_luong");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "than_nhan_nhan_vien",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "than_nhan_nhan_vien",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "la_nguoi_phu_thuoc",
                table: "than_nhan_nhan_vien",
                newName: "LaNguoiPhuThuoc");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "than_nhan_nhan_vien",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "than_nhan_nhan_vien",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "than_nhan_nhan_vien",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_t_nhan_nviens_ma_dinh_danh",
                table: "than_nhan_nhan_vien",
                newName: "IX_than_nhan_nhan_vien_ma_dinh_danh");

            migrationBuilder.RenameIndex(
                name: "ix_t_nhan_nviens_id_mqh",
                table: "than_nhan_nhan_vien",
                newName: "IX_than_nhan_nhan_vien_id_mqh");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "khung_nang_luc_p2",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "khung_nang_luc_p2",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "khung_nang_luc_p2",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "khung_nang_luc_p2",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "khung_nang_luc_p2",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "ix_khung_nang_luc_p2s_id_chuc_vu",
                table: "khung_nang_luc_p2",
                newName: "IX_khung_nang_luc_p2_id_chuc_vu");

            migrationBuilder.AddPrimaryKey(
                name: "vai_tros_pkey",
                table: "vai_tros",
                column: "id_vai_tro");

            migrationBuilder.AddPrimaryKey(
                name: "than_nhans_pkey",
                table: "than_nhans",
                column: "ma_dinh_danh");

            migrationBuilder.AddPrimaryKey(
                name: "tai_khoans_pkey",
                table: "tai_khoans",
                column: "id_tai_khoan");

            migrationBuilder.AddPrimaryKey(
                name: "quyet_dinh_nhan_sus_pkey",
                table: "quyet_dinh_nhan_sus",
                column: "so_quyet_dinh");

            migrationBuilder.AddPrimaryKey(
                name: "phong_bans_pkey",
                table: "phong_bans",
                column: "id_pb");

            migrationBuilder.AddPrimaryKey(
                name: "phieu_danh_gia_nang_lucs_pkey",
                table: "phieu_danh_gia_nang_lucs",
                column: "id_phieu");

            migrationBuilder.AddPrimaryKey(
                name: "phan_cong_cas_pkey",
                table: "phan_cong_cas",
                column: "id_phan_cong");

            migrationBuilder.AddPrimaryKey(
                name: "nhat_ky_trang_thais_pkey",
                table: "nhat_ky_trang_thais",
                column: "id_nhat_ky");

            migrationBuilder.AddPrimaryKey(
                name: "nhan_viens_pkey",
                table: "nhan_viens",
                column: "cccd");

            migrationBuilder.AddPrimaryKey(
                name: "ngay_phep_nhan_viens_pkey",
                table: "ngay_phep_nhan_viens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "ngach_luongs_pkey",
                table: "ngach_luongs",
                column: "id_ngach_luong");

            migrationBuilder.AddPrimaryKey(
                name: "muc_quy_doi_p2s_pkey",
                table: "muc_quy_doi_p2s",
                column: "id_quy_doi");

            migrationBuilder.AddPrimaryKey(
                name: "moi_quan_hes_pkey",
                table: "moi_quan_hes",
                column: "id_mqh");

            migrationBuilder.AddPrimaryKey(
                name: "lich_lam_viecs_pkey",
                table: "lich_lam_viecs",
                column: "id_lich");

            migrationBuilder.AddPrimaryKey(
                name: "ky_luongs_pkey",
                table: "ky_luongs",
                column: "id_ky_luong");

            migrationBuilder.AddPrimaryKey(
                name: "ky_danh_gias_pkey",
                table: "ky_danh_gias",
                column: "id_ky_danh_gia");

            migrationBuilder.AddPrimaryKey(
                name: "khung_gio_nghis_pkey",
                table: "khung_gio_nghis",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "khoan_khau_trus_pkey",
                table: "khoan_khau_trus",
                column: "id_khoan_khau_tru");

            migrationBuilder.AddPrimaryKey(
                name: "hop_dong_lao_dongs_pkey",
                table: "hop_dong_lao_dongs",
                column: "so_hop_dong");

            migrationBuilder.AddPrimaryKey(
                name: "don_nghis_pkey",
                table: "don_nghis",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "chuc_vus_pkey",
                table: "chuc_vus",
                column: "id_chuc_vu");

            migrationBuilder.AddPrimaryKey(
                name: "chi_tiet_lich_lam_viecs_pkey",
                table: "chi_tiet_lich_lam_viecs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "chi_tiet_danh_gia_nang_lucs_pkey",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_chi_tiet");

            migrationBuilder.AddPrimaryKey(
                name: "cham_congs_pkey",
                table: "cham_congs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "cau_hinh_giam_trus_pkey",
                table: "cau_hinh_giam_trus",
                column: "id_cau_hinh_giam_tru");

            migrationBuilder.AddPrimaryKey(
                name: "ca_lam_viecs_pkey",
                table: "ca_lam_viecs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "bang_luongs_pkey",
                table: "bang_luongs",
                column: "id_bang_luong");

            migrationBuilder.AddPrimaryKey(
                name: "bac_thues_pkey",
                table: "bac_thues",
                column: "id_bac_thue");

            migrationBuilder.AddPrimaryKey(
                name: "bac_luongs_pkey",
                table: "bac_luongs",
                column: "id_bac_luong");

            migrationBuilder.AddPrimaryKey(
                name: "than_nhan_nhan_vien_pkey",
                table: "than_nhan_nhan_vien",
                columns: new[] { "cccd", "ma_dinh_danh" });

            migrationBuilder.AddPrimaryKey(
                name: "khung_nang_luc_p2_pkey",
                table: "khung_nang_luc_p2",
                column: "id_tieu_chi");

            migrationBuilder.CreateIndex(
                name: "idx_lich_lam_viec_nam_unique",
                table: "lich_lam_viecs",
                column: "nam",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "bac_luongs_id_ngach_luong_fkey",
                table: "bac_luongs",
                column: "id_ngach_luong",
                principalTable: "ngach_luongs",
                principalColumn: "id_ngach_luong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "bang_luongs_cccd_nhan_vien_fkey",
                table: "bang_luongs",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "bang_luongs_id_ky_luong_fkey",
                table: "bang_luongs",
                column: "id_ky_luong",
                principalTable: "ky_luongs",
                principalColumn: "id_ky_luong",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "cham_congs_cccd_nhan_vien_fkey",
                table: "cham_congs",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "chi_tiet_danh_gias_id_phieu_fkey",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_phieu",
                principalTable: "phieu_danh_gia_nang_lucs",
                principalColumn: "id_phieu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "chi_tiet_danh_gias_id_tieu_chi_fkey",
                table: "chi_tiet_danh_gia_nang_lucs",
                column: "id_tieu_chi",
                principalTable: "khung_nang_luc_p2",
                principalColumn: "id_tieu_chi",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_lich_ca_lam_viec_mac_dinh",
                table: "chi_tiet_lich_lam_viecs",
                column: "id_ca_lam_viec_mac_dinh",
                principalTable: "ca_lam_viecs",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_chi_tiet_lich_lam_viec",
                table: "chi_tiet_lich_lam_viecs",
                column: "id_lich",
                principalTable: "lich_lam_viecs",
                principalColumn: "id_lich",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "chuc_vus_id_chuc_vu_quan_ly_fkey",
                table: "chuc_vus",
                column: "id_chuc_vu_quan_ly",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "chuc_vus_id_ngach_luong_fkey",
                table: "chuc_vus",
                column: "id_ngach_luong",
                principalTable: "ngach_luongs",
                principalColumn: "id_ngach_luong",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "chuc_vus_id_phong_ban_fkey",
                table: "chuc_vus",
                column: "id_phong_ban",
                principalTable: "phong_bans",
                principalColumn: "id_pb",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "don_nghis_cccd_nguoi_duyet_fkey",
                table: "don_nghis",
                column: "cccd_nguoi_duyet",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "don_nghis_cccd_nhan_vien_fkey",
                table: "don_nghis",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "hop_dong_lao_dongs_cccd_fkey",
                table: "hop_dong_lao_dongs",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "khung_gio_nghis_id_ca_lam_viec_fkey",
                table: "khung_gio_nghis",
                column: "id_ca_lam_viec",
                principalTable: "ca_lam_viecs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "khung_nang_luc_id_chuc_vu_fkey",
                table: "khung_nang_luc_p2",
                column: "id_chuc_vu",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ngay_phep_nhan_viens_cccd_fkey",
                table: "ngay_phep_nhan_viens",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "nhan_viens_id_pb_fkey",
                table: "nhan_viens",
                column: "id_pb",
                principalTable: "phong_bans",
                principalColumn: "id_pb",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "nhan_viens_id_tai_khoan_fkey",
                table: "nhan_viens",
                column: "IdTaiKhoan",
                principalTable: "tai_khoans",
                principalColumn: "id_tai_khoan",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "nhat_ky_trang_thais_cccd_fkey",
                table: "nhat_ky_trang_thais",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_phan_cong_ca_ca_lam_viec",
                table: "phan_cong_cas",
                column: "id_ca_lam_viec",
                principalTable: "ca_lam_viecs",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_phan_cong_ca_nhan_vien",
                table: "phan_cong_cas",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "phieu_danh_gias_cccd_nhan_vien_fkey",
                table: "phieu_danh_gia_nang_lucs",
                column: "cccd_nhan_vien",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "phieu_danh_gias_cccd_quan_ly_fkey",
                table: "phieu_danh_gia_nang_lucs",
                column: "cccd_quan_ly",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "phieu_danh_gias_id_ky_danh_gia_fkey",
                table: "phieu_danh_gia_nang_lucs",
                column: "id_ky_danh_gia",
                principalTable: "ky_danh_gias",
                principalColumn: "id_ky_danh_gia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "quyet_dinh_nhan_sus_cccd_fkey",
                table: "quyet_dinh_nhan_sus",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "quyet_dinh_nhan_sus_id_bac_luong_moi_fkey",
                table: "quyet_dinh_nhan_sus",
                column: "id_bac_luong_moi",
                principalTable: "bac_luongs",
                principalColumn: "id_bac_luong",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "quyet_dinh_nhan_sus_id_chuc_vu_moi_fkey",
                table: "quyet_dinh_nhan_sus",
                column: "id_chuc_vu_moi",
                principalTable: "chuc_vus",
                principalColumn: "id_chuc_vu",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "tai_khoans_id_vai_tro_fkey",
                table: "tai_khoans",
                column: "id_vai_tro",
                principalTable: "vai_tros",
                principalColumn: "id_vai_tro",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "than_nhan_nhan_vien_cccd_fkey",
                table: "than_nhan_nhan_vien",
                column: "cccd",
                principalTable: "nhan_viens",
                principalColumn: "cccd",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "than_nhan_nhan_vien_id_mqh_fkey",
                table: "than_nhan_nhan_vien",
                column: "id_mqh",
                principalTable: "moi_quan_hes",
                principalColumn: "id_mqh",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "than_nhan_nhan_vien_ma_dinh_danh_fkey",
                table: "than_nhan_nhan_vien",
                column: "ma_dinh_danh",
                principalTable: "than_nhans",
                principalColumn: "ma_dinh_danh",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
