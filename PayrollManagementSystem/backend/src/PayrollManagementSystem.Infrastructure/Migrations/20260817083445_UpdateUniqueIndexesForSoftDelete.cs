using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUniqueIndexesForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tai_khoans_ten_tai_khoan",
                table: "tai_khoans");

            migrationBuilder.DropIndex(
                name: "ix_phieu_kpis_id_ky_kpi_cccd_nhan_vien",
                table: "phieu_kpis");

            migrationBuilder.DropIndex(
                name: "ix_phan_cong_cas_cccd_nhan_vien_ngay_lam_viec",
                table: "phan_cong_cas");

            migrationBuilder.DropIndex(
                name: "ix_nhan_viens_email",
                table: "nhan_viens");

            migrationBuilder.DropIndex(
                name: "ix_ngay_phep_nhan_viens_cccd_nhan_vien_nam",
                table: "ngay_phep_nhan_viens");

            migrationBuilder.DropIndex(
                name: "ix_ky_luongs_thang_nam",
                table: "ky_luongs");

            migrationBuilder.DropIndex(
                name: "ix_ky_kpis_thang_nam",
                table: "ky_kpis");

            migrationBuilder.DropIndex(
                name: "ix_khoan_khau_trus_ten_khoan_khau_tru",
                table: "khoan_khau_trus");

            migrationBuilder.DropIndex(
                name: "ix_cham_congs_cccd_nhan_vien_ngay_cham_cong",
                table: "cham_congs");

            migrationBuilder.DropIndex(
                name: "ix_bang_luongs_id_ky_luong_cccd_nhan_vien",
                table: "bang_luongs");

            migrationBuilder.CreateIndex(
                name: "ix_tai_khoans_ten_tai_khoan",
                table: "tai_khoans",
                column: "ten_tai_khoan",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_phieu_kpis_id_ky_kpi_cccd_nhan_vien",
                table: "phieu_kpis",
                columns: new[] { "id_ky_kpi", "cccd_nhan_vien" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_phan_cong_cas_cccd_nhan_vien_ngay_lam_viec",
                table: "phan_cong_cas",
                columns: new[] { "cccd_nhan_vien", "ngay_lam_viec" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_nhan_viens_email",
                table: "nhan_viens",
                column: "email",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_ngay_phep_nhan_viens_cccd_nhan_vien_nam",
                table: "ngay_phep_nhan_viens",
                columns: new[] { "cccd_nhan_vien", "nam" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_ky_luongs_thang_nam",
                table: "ky_luongs",
                columns: new[] { "thang", "nam" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_ky_kpis_thang_nam",
                table: "ky_kpis",
                columns: new[] { "thang", "nam" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_khoan_khau_trus_ten_khoan_khau_tru",
                table: "khoan_khau_trus",
                column: "ten_khoan_khau_tru",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_cham_congs_cccd_nhan_vien_ngay_cham_cong",
                table: "cham_congs",
                columns: new[] { "cccd_nhan_vien", "ngay_cham_cong" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_bang_luongs_id_ky_luong_cccd_nhan_vien",
                table: "bang_luongs",
                columns: new[] { "id_ky_luong", "cccd_nhan_vien" },
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tai_khoans_ten_tai_khoan",
                table: "tai_khoans");

            migrationBuilder.DropIndex(
                name: "ix_phieu_kpis_id_ky_kpi_cccd_nhan_vien",
                table: "phieu_kpis");

            migrationBuilder.DropIndex(
                name: "ix_phan_cong_cas_cccd_nhan_vien_ngay_lam_viec",
                table: "phan_cong_cas");

            migrationBuilder.DropIndex(
                name: "ix_nhan_viens_email",
                table: "nhan_viens");

            migrationBuilder.DropIndex(
                name: "ix_ngay_phep_nhan_viens_cccd_nhan_vien_nam",
                table: "ngay_phep_nhan_viens");

            migrationBuilder.DropIndex(
                name: "ix_ky_luongs_thang_nam",
                table: "ky_luongs");

            migrationBuilder.DropIndex(
                name: "ix_ky_kpis_thang_nam",
                table: "ky_kpis");

            migrationBuilder.DropIndex(
                name: "ix_khoan_khau_trus_ten_khoan_khau_tru",
                table: "khoan_khau_trus");

            migrationBuilder.DropIndex(
                name: "ix_cham_congs_cccd_nhan_vien_ngay_cham_cong",
                table: "cham_congs");

            migrationBuilder.DropIndex(
                name: "ix_bang_luongs_id_ky_luong_cccd_nhan_vien",
                table: "bang_luongs");

            migrationBuilder.CreateIndex(
                name: "ix_tai_khoans_ten_tai_khoan",
                table: "tai_khoans",
                column: "ten_tai_khoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_phieu_kpis_id_ky_kpi_cccd_nhan_vien",
                table: "phieu_kpis",
                columns: new[] { "id_ky_kpi", "cccd_nhan_vien" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_phan_cong_cas_cccd_nhan_vien_ngay_lam_viec",
                table: "phan_cong_cas",
                columns: new[] { "cccd_nhan_vien", "ngay_lam_viec" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_nhan_viens_email",
                table: "nhan_viens",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ngay_phep_nhan_viens_cccd_nhan_vien_nam",
                table: "ngay_phep_nhan_viens",
                columns: new[] { "cccd_nhan_vien", "nam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ky_luongs_thang_nam",
                table: "ky_luongs",
                columns: new[] { "thang", "nam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ky_kpis_thang_nam",
                table: "ky_kpis",
                columns: new[] { "thang", "nam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_khoan_khau_trus_ten_khoan_khau_tru",
                table: "khoan_khau_trus",
                column: "ten_khoan_khau_tru",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cham_congs_cccd_nhan_vien_ngay_cham_cong",
                table: "cham_congs",
                columns: new[] { "cccd_nhan_vien", "ngay_cham_cong" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bang_luongs_id_ky_luong_cccd_nhan_vien",
                table: "bang_luongs",
                columns: new[] { "id_ky_luong", "cccd_nhan_vien" },
                unique: true);
        }
    }
}
